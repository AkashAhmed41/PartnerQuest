import { HttpClient } from '@angular/common/http';
import { Injectable } from '@angular/core';
import { environment } from 'src/environments/environment';
import { getPaginatedResult, getPaginationHeaders } from './paginationHelpers';
import { Message } from '../_models/message';
import { HubConnection, HubConnectionBuilder } from '@microsoft/signalr';
import { User } from '../_models/user';
import { BehaviorSubject, take } from 'rxjs';
import { MessageGroup } from '../_models/messageGroup';
import { LoadingIndicatorService } from './loading-indicator.service';

@Injectable({
  providedIn: 'root'
})
export class MessagesService {
  baseUrl = environment.apiUrl;
  hubUrl = environment.hubUrl;
  private hubConnection?: HubConnection;
  private messagesThreadSource = new BehaviorSubject<Message[]>([]);
  messagesThread$ = this.messagesThreadSource.asObservable();

  constructor(private http: HttpClient, private loadingService: LoadingIndicatorService) { }

  createHubConnection(user: User, otherUsername: string) {
    this.loadingService.requestIncoming();
    this.hubConnection = new HubConnectionBuilder()
      .withUrl(this.hubUrl + 'messages?username=' + otherUsername, {
        accessTokenFactory: () => user.token
      })
      .withAutomaticReconnect()
      .build();

    this.hubConnection.start()
      .catch(error => console.log(error))
      .finally(() => this.loadingService.requestCompleted());

    this.hubConnection.on('ReceiveMessagesThread', messages => {
      this.messagesThreadSource.next(messages);
    });

    this.hubConnection.on('UpdatedGroup', (messageGroup: MessageGroup) => {
      if (messageGroup.connections.some(x => x.username === otherUsername)) {
        this.messagesThread$.pipe(take(1)).subscribe({
          next: messages => {
            messages.forEach(message => {
              if (!message.messageRead) {
                message.messageRead = new Date(Date.now());
              }
            });
            this.messagesThreadSource.next([...messages]);
          }
        })
      }
    })

    this.hubConnection.on('NewMessage', message => {
      this.messagesThread$.pipe(take(1)).subscribe({
        next: messages => {
          this.messagesThreadSource.next([...messages, message])
        }
      })
    })
  }

  stopHubConnection() {
    if (this.hubConnection) {
      this.messagesThreadSource.next([]);
      this.hubConnection.stop().catch(err => console.log(err));
    }
  }

  getMessages(pageNumber: number, pageSize: number, container: string) {
    let params = getPaginationHeaders(pageNumber, pageSize);
    params = params.append("Container", container);

    return getPaginatedResult<Message[]>(this.baseUrl + 'messages', params, this.http);
  }

  getMessagesThread(username: string) {
    return this.http.get<Message[]>(this.baseUrl + 'messages/thread/' + username);
  }

  async sendMessage(username: string, content: string) {
    return this.hubConnection?.invoke('SendMessage', { recipientUsername: username, messageContent: content })
      .catch(err => console.log(err));
  }

  deleteMessage(id: number) {
    return this.http.delete(this.baseUrl + 'messages/' + id);
  }

}
