import { CommonModule } from '@angular/common';
import { Component, ViewChild } from '@angular/core';
import { FormsModule, NgForm } from '@angular/forms';

import { LoginService } from '../services/loginService.service';
import { Router } from '@angular/router';
import { Subscription } from 'rxjs';

@Component({
  selector: 'app-login',
  standalone: true,
  imports: [CommonModule, FormsModule],
  templateUrl: './login.component.html',
  styleUrl: './login.component.css',
})
export class LoginComponent {
  @ViewChild('f') loginForm!: NgForm;
  subscription!: Subscription;
  error = null;

  constructor(private loginService: LoginService, private router: Router) {}

  ngOnInit(): void {}

  onSubmit(loginForm: NgForm) {
    const username = loginForm.value.username;
    const psw = loginForm.value.password;

    this.subscription = this.loginService
      .getAuthentication(username)
      .subscribe({
        next: (challenge: string) => {
          this.loginService
            .authentication(username, psw + challenge)
            .subscribe({
              next: () => {
                this.router.navigate(['dashboard/leader-board']);
              },
              error: (e) => {
                // console.log('Error in challenge', e.status, e.error);
                this.error = e.error;
              },
            });
        },
        error: (e) => {
          // console.log('Error in username: ', e.status, e.error);
          this.error = e.error;
        },
      });
  }

  ngOnDestroy(): void {
    this.subscription.unsubscribe();
  }
}
