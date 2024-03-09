import { Component, Input, OnInit } from '@angular/core';
import { ICard } from '../../interfaces/ICard';
import { CommonModule } from '@angular/common';

@Component({
  selector: 'app-card',
  standalone: true,
  imports: [CommonModule],
  templateUrl: './card.component.html',
  styleUrl: './card.component.css',
})
export class CardComponent implements OnInit {
  @Input() card!: ICard;
  @Input() isSelected: boolean = false;

  ngOnInit(): void {}

  getCardColor(): string {
    switch (this.card.color) {
      case 0:
        return '#fc4701'; // ORANGE
      case 1:
        return '#93b900'; // GREEN
      case 2:
        return '#00c9ad'; // AZURE
      case 3:
        return '#fd0076'; //FUCSIA
      default:
        return '#262626'; // DEFAULT BLACK
    }
  }
}
