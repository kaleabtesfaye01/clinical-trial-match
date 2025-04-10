// theme.service.ts
import { Injectable, Renderer2, RendererFactory2 } from '@angular/core';

@Injectable({ providedIn: 'root' })
export class ThemeService {
  private renderer: Renderer2;
  private isDark = false;

  constructor(rendererFactory: RendererFactory2) {
    this.renderer = rendererFactory.createRenderer(null, null);

    const stored = localStorage.getItem('dark-mode');
    if (stored !== null) {
      this.isDark = stored === 'true';
    } else {
      // Use system preference
      const prefersDark = window.matchMedia(
        '(prefers-color-scheme: dark)'
      ).matches;
      this.isDark = prefersDark;
    }

    this.setDarkMode(this.isDark);
  }

  toggleTheme(): void {
    this.setDarkMode(!this.isDark);
  }

  setDarkMode(enable: boolean): void {
    this.isDark = enable;
    localStorage.setItem('dark-mode', String(enable));

    const className = 'dark-mode';
    const body = document.body;

    if (enable) {
      this.renderer.addClass(body, className);
    } else {
      this.renderer.removeClass(body, className);
    }
  }

  isDarkMode(): boolean {
    return this.isDark;
  }

  ngOnInit?() {
    window
      .matchMedia('(prefers-color-scheme: dark)')
      .addEventListener('change', (e) => {
        const prefersDark = e.matches;
        const stored = localStorage.getItem('dark-mode');
        if (!stored) this.setDarkMode(prefersDark);
      });
  }
}
