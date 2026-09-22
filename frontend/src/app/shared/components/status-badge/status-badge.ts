import { Component, computed, input } from '@angular/core';
import { ApplicationStatus, STATUS_LABELS } from '../../../core/models/application.model';

@Component({
  selector: 'app-status-badge',
  standalone: true,
  templateUrl: './status-badge.html',
  styleUrl: './status-badge.scss',
  host: { '[class]': '"status-badge status-badge--" + label().toLowerCase()' },
})
export class StatusBadge {
  public readonly status = input.required<ApplicationStatus>();

  public readonly label = computed(() => STATUS_LABELS[this.status()])
}
