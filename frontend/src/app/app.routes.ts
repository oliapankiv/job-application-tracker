import { Routes } from '@angular/router';
import { authGuard } from './core/modules/auth/guards/auth.guard';

export const routes: Routes = [
  {
    path: 'login',
    loadComponent: () => import('./features/auth/components/login/login').then((m) => m.Login)
  },
  {
    path: 'register',
    loadComponent: () => import('./features/auth/components/register/register').then((m) => m.Register)
  },
  {
    path: '',
    loadComponent: () => import('./shared/layout/components/layout').then((m) => m.Layout),
    canActivate: [authGuard],
    children: [
      { path: '', pathMatch: 'full', redirectTo: 'home' },
      {
        path: 'home',
        loadComponent: () => import('./features/home/components/home').then((m) => m.Home)
      },
      {
        path: 'applications',
        loadComponent: () =>
          import('./features/applications/components/application-list/application-list').then(
            (m) => m.ApplicationList
          )
      },
      {
        path: 'applications/:id',
        loadComponent: () =>
          import('./features/applications/components/application-detail/application-detail').then(
            (m) => m.ApplicationDetail
          )
      },
      {
        path: 'board',
        loadComponent: () =>
          import('./features/applications/components/kanban-board/kanban-board').then((m) => m.KanbanBoard)
      },
      {
        path: 'reminders',
        loadComponent: () => import('./features/reminders/components/reminders/reminders').then((m) => m.Reminders)
      }
    ]
  },
  { path: '**', redirectTo: 'login' }
];
