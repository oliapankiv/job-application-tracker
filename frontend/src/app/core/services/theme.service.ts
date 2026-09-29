import { DOCUMENT } from '@angular/common';
import { Injectable, effect, inject, signal } from '@angular/core';

export type ThemeMode = 'light' | 'dark' | 'system';

const STORAGE_KEY = 'jobapplicationtracker.theme';
const THEME_CLASSES: Record<ThemeMode, string | null> = {
  light: 'theme-light',
  dark: 'theme-dark',
  system: null
};

@Injectable({ providedIn: 'root' })
export class ThemeService {
  private readonly document = inject(DOCUMENT);

  readonly mode = signal<ThemeMode>(this.loadFromStorage());

  constructor() {
    effect(() => {
      const mode = this.mode();
      const root = this.document.documentElement;

      root.classList.remove('theme-light', 'theme-dark');
      const themeClass = THEME_CLASSES[mode];
      if (themeClass) {
        root.classList.add(themeClass);
      }

      try {
        localStorage.setItem(STORAGE_KEY, mode);
      } catch {
        // Storage unavailable; the choice just won't persist.
      }
    });
  }

  setMode(mode: ThemeMode): void {
    this.mode.set(mode);
  }

  private loadFromStorage(): ThemeMode {
    try {
      const stored = localStorage.getItem(STORAGE_KEY);
      if (stored === 'light' || stored === 'dark' || stored === 'system') {
        return stored;
      }
    } catch {
      // Ignore and fall back to the default.
    }
    return 'system';
  }
}
