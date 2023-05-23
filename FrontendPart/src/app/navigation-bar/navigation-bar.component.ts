import { Component, OnInit } from '@angular/core';
import { AccountService } from '../_services/account.service';

@Component({
  selector: 'app-navigation-bar',
  templateUrl: './navigation-bar.component.html',
  styleUrls: ['./navigation-bar.component.css']
})
export class NavigationBarComponent implements OnInit {
  userData: any = {};
  // currentUser$: Observable<User | null> = of(null);

  constructor (public accountService: AccountService) {

  }
  ngOnInit(): void {

  }

  // getCurrentUser() {
  //   this.accountService.currentUser$.subscribe({
  //     next: user => this.loggedIn = !!user,
  //     error: error => console.log(error)
  //   })
  // }

  login() {
    this.accountService.login(this.userData).subscribe({
      next: respone => {
        console.log(respone);
      },
      error: error => console.log(error)
    });
  }

  logout() {
    this.accountService.logout();
  }

}
