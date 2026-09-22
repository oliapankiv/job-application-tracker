import { Component, OnInit, signal } from '@angular/core';
import { Router } from '@angular/router';
import {
  CdkDragDrop,
  DragDropModule,
  moveItemInArray,
  transferArrayItem
} from '@angular/cdk/drag-drop';
import { MatCardModule } from '@angular/material/card';
import { MatIconModule } from '@angular/material/icon';
import { MatProgressSpinnerModule } from '@angular/material/progress-spinner';
import { ApplicationService } from '../../services/application.service';
import { ApplicationStatus, JobApplication, STATUS_LABELS } from '../../models/application.model';
import { ApplicationRoute } from '../../../../shared/features/application/routes/application.route';

interface KanbanColumn {
  status: ApplicationStatus;
  label: string;
  items: JobApplication[];
}

@Component({
  selector: 'app-kanban-board',
  standalone: true,
  imports: [DragDropModule, MatCardModule, MatIconModule, MatProgressSpinnerModule],
  templateUrl: './kanban-board.html',
  styleUrl: './kanban-board.scss'
})
export class KanbanBoard implements OnInit {
  readonly loading = signal(true);
  readonly columns = signal<KanbanColumn[]>([]);

  readonly connectedListIds = Object.values(ApplicationStatus)
    .filter((v) => typeof v === 'number')
    .map((v) => `kanban-column-${v}`);

  constructor(private applicationService: ApplicationService, private router: Router) {}

  ngOnInit(): void {
    this.load();
  }

  load(): void {
    this.loading.set(true);
    this.applicationService.getApplications({ pageSize: 200 }).subscribe((result) => {
      const columns: KanbanColumn[] = Object.entries(STATUS_LABELS).map(([value, label]) => ({
        status: Number(value) as ApplicationStatus,
        label,
        items: result.items.filter((app) => app.status === Number(value))
      }));
      this.columns.set(columns);
      this.loading.set(false);
    });
  }

  dropId(status: ApplicationStatus): string {
    return `kanban-column-${status}`;
  }

  drop(event: CdkDragDrop<JobApplication[]>, targetStatus: ApplicationStatus): void {
    if (event.previousContainer === event.container) {
      moveItemInArray(event.container.data, event.previousIndex, event.currentIndex);
      return;
    }

    const app = event.previousContainer.data[event.previousIndex];
    transferArrayItem(event.previousContainer.data, event.container.data, event.previousIndex, event.currentIndex);

    this.applicationService.updateStatus(app.id, { status: targetStatus }).subscribe({
      error: () => this.load()
    });
    app.status = targetStatus;
  }

  openApplication(app: JobApplication): void {
    this.router.navigate([ApplicationRoute.Detail(app.id)]);
  }
}
