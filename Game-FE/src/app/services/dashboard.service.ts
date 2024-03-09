import { Injectable } from '@angular/core';
import { Observable, map } from 'rxjs';
import { IMatch } from '../interfaces/IMatch';
import { LoginService } from './loginService.service';
import { ApiService } from './apiService.service';
import { ILeaderBoard } from '../interfaces/ILeaderBoard';

@Injectable({
  providedIn: 'root',
})
export class DashboardService {
  constructor(
    private loginService: LoginService,
    private api: ApiService<IMatch[]>
  ) {}

  getLeaderboard(): Observable<ILeaderBoard[]> {
    return this.api
      .get<ILeaderBoard[]>(`/game/match/leaderboard`, {
        withCredentials: true,
      })
      .pipe(
        map((leaderBoardResponse: ILeaderBoard[]) => {
          return leaderBoardResponse;
        })
      );
  }

  getMatchList(): Observable<IMatch[]> {
    return this.api
      .get<IMatch[]>(
        `/game/match/list-match/${this.loginService.player.Username}`,
        {
          withCredentials: true,
        }
      )
      .pipe(
        map((matchListResponse: IMatch[]) => {
          return matchListResponse;
        })
      );
  }

  deleteMatch(matchId: string): Observable<IMatch[]> {
    return this.api
      .delete<IMatch[]>(
        `/game/match/delete/${this.loginService.player.Username}/${matchId}`,
        {
          withCredentials: true,
        }
      )
      .pipe(
        map((deletedResponse: IMatch[]) => {
          return deletedResponse;
        })
      );
  }
}
