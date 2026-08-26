import { CommonModule } from '@angular/common';
import { Component, inject, signal } from '@angular/core';
import { FormsModule } from '@angular/forms';
import { Router } from '@angular/router';

import { AuthService } from '../../../core/services/auth.service';

@Component({
  selector: 'app-login',
  standalone: true,
  imports: [CommonModule, FormsModule],
  templateUrl: './login.html',
  styleUrl: './login.scss'
})
export class LoginComponent {
  private readonly auth = inject(AuthService);
  private readonly router = inject(Router);

  username = 'admin';
  password = 'admin';

  readonly loading = signal(false);
  readonly errorMessage = signal('');

  submit(): void {
    this.loading.set(true);
    this.errorMessage.set('');

    this.auth
      .login({
        username: this.username.trim(),
        password: this.password
      })
      .subscribe({
        next: () => {
          this.loading.set(false);
          void this.router.navigateByUrl('/dashboard');
        },
        error: () => {
          this.loading.set(false);
          this.errorMessage.set(
            'Usuário ou senha inválidos. Use admin/admin, clinician/clinician ou auditor/auditor.'
          );
        }
      });
  }
}
