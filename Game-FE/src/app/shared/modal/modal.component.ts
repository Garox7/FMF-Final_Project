import { CommonModule } from '@angular/common';
import { Component, EventEmitter, Input, Output } from '@angular/core';

@Component({
  selector: 'app-modal',
  standalone: true,
  imports: [CommonModule],
  templateUrl: './modal.component.html',
  styleUrl: './modal.component.css',
})
export class ModalComponent {
  @Input() title!: string;
  @Input() content!: string;
  @Input() actions: { label: string; action: string }[] = [];
  @Output() actionClicked = new EventEmitter<string>();

  onActionClick(action: string) {
    this.actionClicked.emit(action);
  }
}
