import { Component, OnDestroy, OnInit } from '@angular/core';
import { CardComponent } from '../card/card.component';
import { CommonModule } from '@angular/common';

import { ICard } from '../../interfaces/ICard';
import { MatchService } from '../../services/match.service';
import { IMatch } from '../../interfaces/IMatch';
import { Subscription } from 'rxjs';
import { IPlayer } from '../../interfaces/IPlayer';

@Component({
  selector: 'app-player',
  standalone: true,
  templateUrl: './player.component.html',
  styleUrl: './player.component.css',
  imports: [CommonModule, CardComponent],
})
export class PlayerComponent implements OnInit, OnDestroy {
  player!: IPlayer;
  score!: number;
  subscription!: Subscription;
  selectedCards: ICard[] = [];
  isDiscardComplete = false;

  constructor(private matchService: MatchService) {}

  ngOnInit(): void {
    this.subscription = this.matchService.matchChange.subscribe(
      (matchRes: IMatch) => {
        this.player = matchRes.player;
        this.score = matchRes.playerScore;
      }
    );

    // console.log('In Player Hand Component: ', this.player); // DEBUG
  }

  onDiscard(card: ICard) {
    const index = this.selectedCards.indexOf(card);
    if (index !== -1) {
      this.selectedCards.splice(index, 1);
    } else if (!this.isDiscardComplete) {
      this.selectedCards.push(card);
    } else {
      this.matchService.errorMessageChange.next(
        'You cannot select more than two cards'
      );
    }

    this.isDiscardComplete = this.selectedCards.length === 2;

    // console.log('Selected Cards: ', this.selectedCards); // DEBUG
  }

  onMakeMove() {
    this.matchService.addDiscardCard(this.selectedCards);
    this.selectedCards = [];
    this.isDiscardComplete = false;
    this.matchService.playTurn().subscribe({
      // next: (matchResponse: IMatch) => {
      //   console.log(matchResponse);
      // },
      error: (err) => {
        this.matchService.errorMessageChange.next(err.error);
      },
    });
  }

  ngOnDestroy(): void {
    this.subscription.unsubscribe();
  }
}
