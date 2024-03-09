import { CommonModule } from '@angular/common';
import { Component, OnInit } from '@angular/core';

import { DashboardService } from '../../services/dashboard.service';
import { ILeaderBoard } from '../../interfaces/ILeaderBoard';

@Component({
  selector: 'app-leader-board',
  standalone: true,
  imports: [CommonModule],
  templateUrl: './leader-board.component.html',
  styleUrl: './leader-board.component.css',
})
export class LeaderBoardComponent implements OnInit {
  leaderBoards!: ILeaderBoard[];

  constructor(private dashboardService: DashboardService) {}

  ngOnInit(): void {
    this.dashboardService.getLeaderboard().subscribe({
      next: (leaderBoardRes: ILeaderBoard[]) => {
        this.leaderBoards = leaderBoardRes;
      },
    });
  }
}
