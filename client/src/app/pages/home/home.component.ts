import { Component } from '@angular/core';
import { CommonModule } from '@angular/common';
import { RouterModule } from '@angular/router';
import { MatButtonModule } from '@angular/material/button';
import { MatIconModule } from '@angular/material/icon';
import { MatCardModule } from '@angular/material/card';

@Component({
  selector: 'app-home',
  standalone: true,
  imports: [
    CommonModule,
    RouterModule,
    MatButtonModule,
    MatIconModule,
    MatCardModule
  ],
  templateUrl: './home.component.html',
  styleUrls: ['./home.component.scss']
})
export class HomeComponent {
  currentYear = new Date().getFullYear();
  
  features = [
    {
      icon: 'search',
      title: 'Smart Matching',
      description: 'Our AI-powered system analyzes patient data to find the most relevant clinical trials.'
    },
    {
      icon: 'location_on',
      title: 'Location-Based',
      description: 'Find trials near you with our location-aware search capabilities.'
    },
    {
      icon: 'update',
      title: 'Real-Time Updates',
      description: 'Access the latest clinical trial information, updated in real-time.'
    },
    {
      icon: 'verified_user',
      title: 'Verified Trials',
      description: 'All trials are verified and sourced from ClinicalTrials.gov.'
    }
  ];

  howItWorks = [
    {
      step: 1,
      title: 'Enter Patient Information',
      description: 'Provide basic patient details and medical history.'
    },
    {
      step: 2,
      title: 'AI-Powered Matching',
      description: 'Our system analyzes the data to find relevant trials.'
    },
    {
      step: 3,
      title: 'Review Matches',
      description: 'Browse through matched trials with detailed information.'
    },
    {
      step: 4,
      title: 'Take Action',
      description: 'Contact trial coordinators or save trials for later.'
    }
  ];
}
