import { Component, computed, inject } from '@angular/core';
import {
  RouterLink,
  RouterLinkActive,
  RouterOutlet
} from '@angular/router';

import { AuthService } from '../../core/services/auth.service';

@Component({
  selector: 'app-shell',
  standalone: true,
  imports: [
    RouterOutlet,
    RouterLink,
    RouterLinkActive
  ],
  templateUrl: './shell.html',
  styleUrl: './shell.scss'
})
export class Shell {
  private readonly auth = inject(AuthService);

  readonly username = this.auth.username;
  readonly rolesLabel = computed(() => {
    const roles = this.auth.roles();
    return roles.length ? roles.join(' · ') : 'Hospital Intelligence';
  });
  readonly avatarLetter = computed(() => {
    const name = this.username();
    return name ? name.charAt(0).toUpperCase() : '?';
  });

  logout(): void {
    this.auth.logout();
  }
}
