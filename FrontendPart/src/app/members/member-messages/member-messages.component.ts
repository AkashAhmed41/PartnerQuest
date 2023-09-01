import { Component, Input, OnInit } from '@angular/core';
import { Message } from 'src/app/_models/message';
import { MessagesService } from 'src/app/_services/messages.service';

@Component({
  selector: 'app-member-messages',
  templateUrl: './member-messages.component.html',
  styleUrls: ['./member-messages.component.css']
})
export class MemberMessagesComponent implements OnInit {
  @Input() username?: string;
  messages: Message[] = [];

  constructor(private messageService: MessagesService) {}

  ngOnInit(): void {
    this.loadMessagesThread();
  }

  loadMessagesThread() {
    if (this.username) {
      this.messageService.getMessagesThread(this.username).subscribe({
        next: messages => this.messages = messages
      });
    }
  }

}
