import { Injectable } from '@angular/core';
import { Observable, map } from 'rxjs';

import { ApiService } from './apiService.service';

interface PlayerDto {
  Id: number;
  Username: string;
  Name: string;
  Surname: string;
  Token: string;
}

@Injectable({
  providedIn: 'root',
})
export class LoginService {
  player!: PlayerDto;

  constructor(private apiService: ApiService<string>) {
    const playerData = localStorage.getItem('player');

    if (playerData) {
      this.player = JSON.parse(playerData);
    }
  }

  getAuthentication(username: string): Observable<string> {
    return this.apiService.get<string>(`/login/${username}`).pipe(
      map((challenge) => {
        return challenge;
      })
    );
  }

  authentication(username: string, challenge: string): Observable<PlayerDto> {
    return this.apiService
      .get<PlayerDto>(`/login/${username}/${challenge}`)
      .pipe(
        map((playerData: PlayerDto) => {
          this.player = playerData;
          localStorage.setItem('player', JSON.stringify(playerData));
          localStorage.setItem('token', this.player.Token);
          return playerData;
        })
      );
  }

  checkAuthentication(): boolean {
    const token = localStorage.getItem('token');

    if (token) {
      return true;
    }
    return false;
  }
}
