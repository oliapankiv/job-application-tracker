import { Component } from '@angular/core';
import { RouterLink, RouterLinkActive, RouterOutlet } from '@angular/router';
import { MatIconModule } from '@angular/material/icon';
import { MatListModule } from '@angular/material/list';
import { MatSidenavModule } from '@angular/material/sidenav';
import { MatToolbarModule } from '@angular/material/toolbar';
import { MatButtonModule } from '@angular/material/button';
import { AuthService } from '../../../core/modules/auth/services/auth.service';
import { HomeRoute } from '../../features/home/routes/home.route';
import { ApplicationsRoute } from '../../features/applications/routes/applications.route';
import { RemindersRoute } from '../../features/reminders/routes/reminders.route';

@Component({
  selector: 'app-shell',
  standalone: true,
  imports: [
    RouterOutlet,
    RouterLink,
    RouterLinkActive,
    MatSidenavModule,
    MatToolbarModule,
    MatListModule,
    MatIconModule,
    MatButtonModule
  ],
  templateUrl: './shell.html',
  styleUrl: './shell.scss'
})
export class Shell {
  readonly navLinks = [
    { path: HomeRoute.Main, label: 'Home', icon: 'home' },
    { path: ApplicationsRoute.Main, label: 'Applications', icon: 'list_alt' },
    { path: ApplicationsRoute.Board, label: 'Kanban Board', icon: 'view_kanban' },
    { path: RemindersRoute.Main, label: 'Reminders', icon: 'notifications' }
  ];

  constructor(public authService: AuthService) {}

  logout(): void {
    this.authService.logout();
  }
}
