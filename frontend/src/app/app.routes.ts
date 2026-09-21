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
    path: 'home',
    loadComponent: () => import('./features/home/components/home').then((m) => m.Home),
    canActivate: [authGuard]
  },
  { path: '', pathMatch: 'full', redirectTo: 'login' },
  { path: '**', redirectTo: 'login' }
];
