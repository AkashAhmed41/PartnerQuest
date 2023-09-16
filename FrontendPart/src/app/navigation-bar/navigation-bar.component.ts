import { Component, OnInit } from '@angular/core';
import { AccountService } from '../_services/account.service';
import { Router } from '@angular/router';

@Component({
  selector: 'app-navigation-bar',
  templateUrl: './navigation-bar.component.html',
  styleUrls: ['./navigation-bar.component.css']
})
export class NavigationBarComponent implements OnInit {
  userData: any = {};
  // currentUser$: Observable<User | null> = of(null);

  constructor (public accountService: AccountService, private router: Router) {

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
      next: response => {
        this.router.navigateByUrl('/members');
        this.userData = {};
        console.log(response);
      },
      error: error => {
        console.log(error);
      }
    });
  }

  logout() {
    this.accountService.logout();
    this.router.navigateByUrl('/');
  }

}
