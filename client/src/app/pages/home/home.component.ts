import { Component } from '@angular/core';
import { Router } from '@angular/router';
import { MatSnackBar } from '@angular/material/snack-bar';
import {
  trigger,
  style,
  animate,
  transition,
  query,
  stagger,
} from '@angular/animations';
import { MatExpansionModule } from '@angular/material/expansion';
import { MatIcon } from '@angular/material/icon';
import { SvgIconComponent } from '../../shared/components/svg-icon/svg-icon.component';
import { NavMenuComponent } from '../../shared/components/nav-menu/nav-menu.component';
import { CommonModule } from '@angular/common';

@Component({
  selector: 'app-home',
  imports: [
    MatExpansionModule,
    MatIcon,
    SvgIconComponent,
    NavMenuComponent,
    CommonModule,
  ],
  standalone: true,
  templateUrl: './home.component.html',
  styleUrls: ['./home.component.scss'],
  animations: [
    trigger('fadeIn', [
      transition(':enter', [
        style({ opacity: 0, transform: 'translateY(12px)' }),
        animate('400ms ease-out', style({ opacity: 1, transform: 'none' })),
      ]),
    ]),
    trigger('staggeredFadeIn', [
      transition(':enter', [
        query(
          '.feature, .step-card',
          [
            style({ opacity: 0, transform: 'translateY(20px)' }),
            stagger(
              150,
              animate(
                '400ms ease-out',
                style({ opacity: 1, transform: 'none' })
              )
            ),
          ],
          { optional: true }
        ),
      ]),
    ]),
  ],
})
export class HomeComponent {
  currentYear = new Date().getFullYear();

  howItWorks = [
    {
      step: 1,
      title: 'Share Your Info',
      description:
        'Provide details about your health condition and preferences.',
    },
    {
      step: 2,
      title: 'Get Matched',
      description:
        'Our AI scans thousands of trials to find your best options.',
    },
    {
      step: 3,
      title: 'Explore & Connect',
      description:
        'Review trial details and connect with coordinators securely.',
    },
  ];

  faqs = [
    {
      q: 'Is my personal data safe?',
      a: 'Absolutely. Your information is encrypted and never shared without consent.',
    },
    {
      q: 'How accurate is the matching?',
      a: 'Our AI engine is trained on real clinical data for high-quality results.',
    },
    {
      q: 'Is this service free?',
      a: 'Yes, it’s completely free for patients.',
    },
  ];

  constructor(private router: Router, private snackBar: MatSnackBar) {}

  startSearch(): void {
    this.snackBar.open('Let’s match you with clinical trials!', 'Close', {
      duration: 3000,
      horizontalPosition: 'center',
      verticalPosition: 'top',
      panelClass: ['mat-primary'],
    });

    setTimeout(() => {
      this.router.navigate(['/patient-form']);
    }, 800);
  }
}
