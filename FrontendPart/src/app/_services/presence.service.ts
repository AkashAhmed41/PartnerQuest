import { Injectable } from '@angular/core';
import { HubConnection, HubConnectionBuilder } from '@microsoft/signalr';
import { ToastrService } from 'ngx-toastr';
import { environment } from 'src/environments/environment';
import { User } from '../_models/user';
import { BehaviorSubject, filter, take } from 'rxjs';
import { Router } from '@angular/router';

@Injectable({
  providedIn: 'root'
})
export class PresenceService {
  hubUrl = environment.hubUrl;
  private hubConnection?: HubConnection;
  private activeUsersSource = new BehaviorSubject<string[]>([]);
  activeUsers$ = this.activeUsersSource.asObservable();

  constructor(private toastr: ToastrService, private router: Router) { }

  createHubConnection(user: User) {
    this.hubConnection = new HubConnectionBuilder()
      .withUrl(this.hubUrl + 'presence', {
        accessTokenFactory: () => user.token
      })
      .withAutomaticReconnect()
      .build();

    this.hubConnection.start().catch(error => console.log(error));

    this.hubConnection.on('UserIsOnline', username => {
      this.activeUsers$.pipe(take(1)).subscribe({
        next: usernames => this.activeUsersSource.next([...usernames, username])
      })
    });

    this.hubConnection.on('UserIsOffline', username => {
      this.activeUsers$.pipe(take(1)).subscribe({
        next: usernames => this.activeUsersSource.next(usernames.filter(x => x !== username))
      })
    });

    this.hubConnection.on('GetActiveUsers', activeUsersNames => {
      this.activeUsersSource.next(activeUsersNames);
    });

    this.hubConnection.on('NewMessageReceived', ({ username, nickname }) => {
      this.toastr.info(`You have new message from ${nickname}!\nClick to open...`)
        .onTap.pipe(take(1)).subscribe({
          next: () => this.router.navigateByUrl('/members/' + username + '?tab=Messages')
        })
    });
  }

  stopHubConnection() {
    this.hubConnection?.stop().catch(error => console.log(error));
  }

}
