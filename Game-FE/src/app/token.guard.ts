import { CanActivateFn, Router } from '@angular/router';
import { LoginService } from './services/loginService.service';
import { inject } from '@angular/core';

export const tokenGuard: CanActivateFn = (route, state) => {
  if (!inject(LoginService).checkAuthentication()) {
    inject(Router).navigate(['']);
    return false;
  } else {
    return true;
  }
};
