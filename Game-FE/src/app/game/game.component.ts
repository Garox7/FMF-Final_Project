import { CommonModule } from '@angular/common';
import { Component, OnDestroy, OnInit } from '@angular/core';
import { ActivatedRoute, Router } from '@angular/router';

import { ComputerPlayerComponent } from './computer-player/computer-player.component';
import { DecksComponent } from './decks/decks.component';
import { PlayerComponent } from './player/player.component';
import { IMatch } from '../interfaces/IMatch';
import { MatchService } from '../services/match.service';
import { ModalComponent } from '../shared/modal/modal.component';
import { Subscription } from 'rxjs';

@Component({
  selector: 'app-game',
  standalone: true,
  templateUrl: './game.component.html',
  styleUrl: './game.component.css',
  imports: [
    CommonModule,
    ComputerPlayerComponent,
    DecksComponent,
    PlayerComponent,
    ModalComponent,
  ],
})
export class GameComponent implements OnInit, OnDestroy {
  match!: IMatch;
  modal: boolean = false;
  modalTitle!: string;
  modalContent!: string;
  modalActions: { label: string; action: string }[] = [];
  subscription!: Subscription;

  constructor(
    private matchService: MatchService,
    private router: Router,
    private route: ActivatedRoute
  ) {}

  ngOnInit(): void {
    const matchId: string = this.route.snapshot.params['id'];

    setTimeout(() => {
      const matchSub: Subscription = this.matchService
        .getMatch(matchId)
        .subscribe({
          next: (matchRes: IMatch) => {
            this.match = matchRes;

            // console.log('Match Response: ', matchRes);
          },
          complete: () => {
            matchSub.unsubscribe();
          },
        });
    }, 600);

    setTimeout(() => {
      this.subscription = this.matchService.matchChange.subscribe({
        next: (matchRes: IMatch) => {
          if (this.match && matchRes.round != this.match.round) {
            this.onChangeRound(this.match, matchRes);
            this.match = matchRes;
          } else if (this.match && matchRes.status != 0) {
            this.onCompleteMatch(matchRes);
            // this.match = matchRes;
          } else {
            this.match = matchRes;
          }
        },
      });
    }, 600);
  }

  onExit() {
    this.modal = true;
    this.modalTitle = 'Are you sure you want to exit the game?';
    this.modalContent =
      'The game will be saved at this point and you can resume it next time';
    this.modalActions = [
      { label: 'Exit', action: 'exit' },
      { label: 'Cancel', action: 'cancel' },
    ];
  }

  handleModalAction(action: string) {
    if (action === 'exit') {
      this.router.navigate(['dashboard/matches']);
    } else if (action === 'cancel') {
      this.modal = false;
    }
  }

  onChangeRound(oldMatch: IMatch, newMatch: IMatch) {
    this.modal = true;
    this.modalTitle = `Round ${oldMatch.round} concluded`;
    this.modalContent =
      `Your Score: ${newMatch.playerScore} ` +
      '\n' +
      `Computer Score: ${newMatch.computerScore}`;
    this.modalActions = [{ label: 'Next Round!', action: 'cancel' }];
  }

  onCompleteMatch(newMatch: IMatch) {
    this.modal = true;
    this.modalTitle =
      newMatch.status == 1 ? 'Congratulations, you won' : 'You lost!';
    this.modalContent = '';
    this.modalActions = [{ label: 'Returns to the dashboard', action: 'exit' }];
  }

  ngOnDestroy(): void {
    this.subscription.unsubscribe();
  }
}
