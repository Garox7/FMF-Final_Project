import { CommonModule } from '@angular/common';
import { Component, OnInit } from '@angular/core';
import { Router, RouterLink, RouterOutlet } from '@angular/router';

import { MatchService } from '../services/match.service';
import { IMatch } from '../interfaces/IMatch';
import { NavBarComponent } from './nav-bar/nav-bar.component';
import { LoginService } from '../services/loginService.service';

@Component({
  selector: 'app-dash-board',
  standalone: true,
  templateUrl: './dash-board.component.html',
  styleUrl: './dash-board.component.css',
  imports: [CommonModule, RouterOutlet, RouterLink, NavBarComponent],
})
export class DashBoardComponent implements OnInit {
  playerUsername!: string;
  constructor(
    private loginService: LoginService,
    private matchService: MatchService,
    private router: Router
  ) {}

  ngOnInit(): void {
    this.playerUsername = this.loginService.player.Username;
  }

  onLogout() {
    localStorage.clear();
    this.router.navigate(['']);
  }

  onNewMatch() {
    this.matchService.newMatch().subscribe({
      next: (matchResponse: IMatch) => {
        this.router.navigate(['game', matchResponse.id]);

        // console.log(matchResponse); // DEBUG
      },
    });
  }
}
