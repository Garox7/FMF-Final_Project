import { CommonModule } from '@angular/common';
import { Component, Input, OnDestroy, OnInit } from '@angular/core';

import { ICard } from '../../interfaces/ICard';
import { CardComponent } from '../card/card.component';
import { MatchService } from '../../services/match.service';
import { Subscription } from 'rxjs';

@Component({
  selector: 'app-decks',
  standalone: true,
  templateUrl: './decks.component.html',
  styleUrl: './decks.component.css',
  imports: [CommonModule, CardComponent],
})
export class DecksComponent implements OnInit, OnDestroy {
  @Input() discardCards!: [ICard[], ICard[]];
  selectedPileIndex: number = 0;
  errorMessage!: string;
  showError: boolean = false;
  subscription!: Subscription;

  constructor(private matchService: MatchService) {}

  ngOnInit(): void {
    this.subscription = this.matchService.errorMessageChange.subscribe(
      (errorRes: string) => {
        this.showErrorMessage(errorRes);
      }
    );
  }

  onDrawCard() {
    this.matchService.getCard().subscribe({
      error: (errorResponse) => {
        this.showErrorMessage(errorResponse.error);
      },
    });
  }

  showErrorMessage(message: string) {
    this.errorMessage = message;
    this.showError = true;
    setTimeout(() => {
      this.showError = false;
    }, 4000);
  }

  onSelectPileIndex(pileId: number) {
    if (this.selectedPileIndex != pileId) {
      this.selectedPileIndex = pileId;
      this.matchService.updatePileId(pileId);
    } else {
      this.selectedPileIndex = 0;
      this.matchService.updatePileId(0);
    }
  }

  ngOnDestroy(): void {
    this.subscription.unsubscribe();
  }
}
