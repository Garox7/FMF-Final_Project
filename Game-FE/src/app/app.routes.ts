import { Routes } from '@angular/router';
import { LoginComponent } from './login/login.component';
import { DashBoardComponent } from './dash-board/dash-board.component';
import { tokenGuard } from './token.guard';
import { GameComponent } from './game/game.component';
import { MatchesComponent } from './dash-board/matches/matches.component';
import { LeaderBoardComponent } from './dash-board/leader-board/leader-board.component';

export const routes: Routes = [
  {
    path: '',
    redirectTo: '/login',
    pathMatch: 'full',
  },
  {
    path: 'login',
    component: LoginComponent,
  },
  {
    path: 'dashboard',
    component: DashBoardComponent,
    canActivate: [tokenGuard],
    children: [
      {
        path: 'leader-board',
        component: LeaderBoardComponent,
        canActivate: [tokenGuard],
      },
      {
        path: 'matches',
        component: MatchesComponent,
        canActivate: [tokenGuard],
      },
    ],
  },
  {
    path: 'game/:id',
    component: GameComponent,
    canActivate: [tokenGuard],
  },
];
