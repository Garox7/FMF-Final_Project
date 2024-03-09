import { Component, OnDestroy, OnInit } from '@angular/core';
import { Subscription } from 'rxjs';
import { CommonModule } from '@angular/common';
import { Router } from '@angular/router';

import { ModalComponent } from '../../shared/modal/modal.component';
import { IMatch } from '../../interfaces/IMatch';
import { DashboardService } from '../../services/dashboard.service';

@Component({
  selector: 'app-matches',
  standalone: true,
  templateUrl: './matches.component.html',
  styleUrl: './matches.component.css',
  imports: [CommonModule, ModalComponent],
})
export class MatchesComponent implements OnInit, OnDestroy {
  matches!: IMatch[];
  tempMatchId!: string;
  modal: boolean = false;
  modalTitle!: string;
  modalContent!: string;
  modalActions: { label: string; action: string }[] = [];
  subscription!: Subscription;

  constructor(
    private dashboardService: DashboardService,
    private router: Router
  ) {}

  ngOnInit(): void {
    this.subscription = this.dashboardService
      .getMatchList()
      .subscribe((matchesResponse: IMatch[]) => {
        this.matches = matchesResponse;

        // console.log('match list', matchesResponse); // DEBUG
      });
  }

  onMatch(id: string) {
    this.router.navigate(['game', id]);
  }

  onDeleteMatch(id: string) {
    this.tempMatchId = id;
    this.modal = true;
    this.modalTitle = 'Are you sure you want to delete this match?';
    this.modalContent =
      'The action is irreversible and you will no longer be able to recover it in the future';
    this.modalActions = [
      { label: 'Delete', action: 'delete' },
      { label: 'Cancel', action: 'cancel' },
    ];
  }

  handleModalAction(action: string) {
    if (action === 'delete') {
      this.dashboardService.deleteMatch(this.tempMatchId).subscribe({
        next: (matchesResponse: IMatch[]) => {
          this.matches = matchesResponse;
          this.modalTitle = 'You have successfully eliminated the match';
          this.modalContent = '';
          this.modalActions = [{ label: 'Continue', action: 'cancel' }];
        },
        error: (err) => {
          this.modalTitle = err.error;
          this.modalContent = '';
          this.modalActions = [{ label: 'Continue', action: 'cancel' }];
        },
      });
    } else if (action === 'cancel') {
      this.modal = false;
    }
  }

  ngOnDestroy(): void {
    this.subscription.unsubscribe();
  }
}
