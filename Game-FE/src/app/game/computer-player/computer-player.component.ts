import { CommonModule } from '@angular/common';
import { Component, OnDestroy, OnInit } from '@angular/core';
import { Subscription } from 'rxjs';

import { MatchService } from '../../services/match.service';
import { IMatch } from '../../interfaces/IMatch';

@Component({
  selector: 'app-computer-player',
  standalone: true,
  imports: [CommonModule],
  templateUrl: './computer-player.component.html',
  styleUrl: './computer-player.component.css',
})
export class ComputerPlayerComponent implements OnInit, OnDestroy {
  score!: number;
  numberOfCards: number = 7;
  initialHand = Array(this.numberOfCards);
  subcription!: Subscription;

  constructor(private matchService: MatchService) {}

  ngOnInit(): void {
    this.subcription = this.matchService.matchChange.subscribe(
      (matchResponse: IMatch) => {
        this.score = matchResponse.computerScore;
        this.numberOfCards = matchResponse.computerHandSize;
        this.initialHand = new Array(this.numberOfCards);
      }
    );
  }

  ngOnDestroy(): void {
    this.subcription.unsubscribe();
  }
}
