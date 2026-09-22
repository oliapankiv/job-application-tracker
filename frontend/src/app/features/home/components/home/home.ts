import { Component, OnInit, inject, signal } from '@angular/core';
import { NgxChartsModule, Color, ScaleType } from '@swimlane/ngx-charts';
import { MatCardModule } from '@angular/material/card';
import { MatProgressSpinnerModule } from '@angular/material/progress-spinner';
import { AuthService } from '../../../../core/modules/auth/services/auth.service';
import { DashboardService } from '../../../dashboard/services/dashboard.service';
import { DashboardStats } from '../../../../shared/features/dashboard/models/dashboard.model';

interface ChartDatum {
  name: string;
  value: number;
}

const STATUS_COLOR_MAP: Record<string, string> = {
  Applied: '#86b6ef',
  Screening: '#5598e7',
  Interview: '#2a78d6',
  Offer: '#1c5cab',
  Rejected: '#d03b3b'
};

@Component({
  selector: 'app-home',
  standalone: true,
  imports: [MatCardModule, MatProgressSpinnerModule, NgxChartsModule],
  templateUrl: './home.html',
  styleUrl: './home.scss'
})
export class Home implements OnInit {
  private authService = inject(AuthService);
  private dashboardService = inject(DashboardService);

  readonly currentUser = this.authService.currentUser;

  readonly loading = signal(true);
  readonly stats = signal<DashboardStats | null>(null);
  readonly weeklyData = signal<ChartDatum[]>([]);
  readonly statusData = signal<ChartDatum[]>([]);

  readonly weeklyColorScheme: Color = {
    name: 'weekly',
    selectable: false,
    group: ScaleType.Ordinal,
    domain: ['#2a78d6']
  };

  readonly statusColorScheme = signal<Color>({
    name: 'status',
    selectable: false,
    group: ScaleType.Ordinal,
    domain: []
  });

  ngOnInit(): void {
    this.dashboardService.getStats().subscribe((stats) => {
      this.stats.set(stats);
      this.weeklyData.set(
        stats.applicationsPerWeek.map((w) => ({
          name: this.formatWeekLabel(w.weekStart),
          value: w.count
        }))
      );

      const statusEntries = stats.statusBreakdown.filter((s) => s.count > 0);
      this.statusData.set(statusEntries.map((s) => ({ name: s.status, value: s.count })));
      this.statusColorScheme.set({
        name: 'status',
        selectable: false,
        group: ScaleType.Ordinal,
        domain: statusEntries.map((s) => STATUS_COLOR_MAP[s.status] ?? '#2a78d6')
      });

      this.loading.set(false);
    });
  }

  private formatWeekLabel(iso: string): string {
    const date = new Date(iso);
    return date.toLocaleDateString(undefined, { month: 'short', day: 'numeric' });
  }
}
