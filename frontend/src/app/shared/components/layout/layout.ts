import { Component, computed } from '@angular/core';
import { RouterLink, RouterLinkActive, RouterOutlet } from '@angular/router';
import { MatIconModule } from '@angular/material/icon';
import { MatListModule } from '@angular/material/list';
import { MatSidenavModule } from '@angular/material/sidenav';
import { MatToolbarModule } from '@angular/material/toolbar';
import { MatButtonModule } from '@angular/material/button';
import { MatMenuModule } from '@angular/material/menu';
import { ThemeMode, ThemeService } from '../../../core/services/theme.service';
import { AuthService } from '../../../core/modules/auth/services/auth.service';
import { HomeRoute } from '../../features/home/routes/home.route';
import { ApplicationRoute } from '../../features/application/routes/application.route';
import { ReminderRoute } from '../../features/reminder/routes/reminder.route';

@Component({
  selector: 'app-layout',
  standalone: true,
  imports: [
    RouterOutlet,
    RouterLink,
    RouterLinkActive,
    MatSidenavModule,
    MatToolbarModule,
    MatListModule,
    MatIconModule,
    MatButtonModule,
    MatMenuModule
  ],
  templateUrl: './layout.html',
  styleUrl: './layout.scss'
})
export class Layout {
  readonly navLinks = [
    { path: HomeRoute.Main, label: 'Home', icon: 'home' },
    { path: ApplicationRoute.Main, label: 'Applications', icon: 'list_alt' },
    { path: ApplicationRoute.Board, label: 'Kanban Board', icon: 'view_kanban' },
    { path: ReminderRoute.Main, label: 'Reminders', icon: 'notifications' }
  ];

  readonly themeOptions: { mode: ThemeMode; label: string; icon: string }[] = [
    { mode: 'light', label: 'Light', icon: 'light_mode' },
    { mode: 'dark', label: 'Dark', icon: 'dark_mode' },
    { mode: 'system', label: 'System', icon: 'brightness_auto' }
  ];

  readonly currentThemeIcon = computed(
    () => this.themeOptions.find((option) => option.mode === this.themeService.mode())!.icon
  );

  constructor(public authService: AuthService, public themeService: ThemeService) {}

  logout(): void {
    this.authService.logout();
  }
}
