import { Component, OnInit } from '@angular/core';
import { AccountService } from '../_services/account.service';

@Component({
  selector: 'app-navigation-bar',
  templateUrl: './navigation-bar.component.html',
  styleUrls: ['./navigation-bar.component.css']
})
export class NavigationBarComponent implements OnInit {
  userData: any = {};
  loggedIn = false;

  constructor (private accountService: AccountService) {

  }
  ngOnInit(): void {
    this.getCurrentUser();
  }

  getCurrentUser() {
    this.accountService.currentUser$.subscribe({
      next: user => this.loggedIn = !!user,
      error: error => console.log(error)
    })
  }

  login() {
    this.accountService.login(this.userData).subscribe({
      next: respone => {
        console.log(respone);
        this.loggedIn = true;
      },
      error: error => console.log(error)
    });
  }

  logout() {
    this.accountService.logout();
    this.loggedIn = false;
  }

}
