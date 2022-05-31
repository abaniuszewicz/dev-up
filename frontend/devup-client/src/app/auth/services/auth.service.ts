import { HttpClient, HttpHeaders } from '@angular/common/http';
import { Injectable } from '@angular/core';
import { Router } from '@angular/router';
import { BehaviorSubject, map, Observable } from 'rxjs';
import { environment } from 'src/environments/environment';
import { Identity } from '../models/identity';
import { LoginRequest } from '../models/login-request';
import { RefreshRequest } from '../models/refresh-request';
import { RegisterRequest } from '../models/register-request';

@Injectable({
  providedIn: 'root',
})
export class AuthService {
  userSubject$: BehaviorSubject<Identity>;
  public user$: Observable<Identity>;
  private readonly empty: any = {};
  private readonly emptyUser: Identity = { ...this.empty };
  private readonly url: string = environment.apiUrlSsl;
  private readonly uri: string = 'identity';

  private readonly header = new HttpHeaders()
    .set('content-type', 'application/json')
    .set('Access-Control-Allow-Origin', '*');

  constructor(private router: Router, private httpClient: HttpClient) {
    this.userSubject$ = new BehaviorSubject<Identity>(
      JSON.parse(localStorage.getItem('user') ?? '{}')
    );

    this.user$ = this.userSubject$.asObservable();
  }

  get currentUser(): Identity {
    return this.userSubject$.getValue();
  }

  get isLoggedIn(): boolean {
    const user: Identity = JSON.parse(localStorage.getItem('user') ?? '{}');
    if (user && user.token) {
      return true;
    }

    return false;
  }

  login(loginInfo: LoginRequest) {
    this.httpClient
      .post<Identity>(`${this.url}/${this.uri}/login`, loginInfo, {
        headers: this.header,
      })
      .subscribe({
        next: (x) => {
          this.userSubject$.next({
            token: x.token,
            refreshToken: x.refreshToken,
          });

          localStorage.setItem('user', JSON.stringify(x));
          this.router.navigate(['/profile']);

          return x;
        },
        error: (e) => this.errorAlert(e),
      });
  }

  register(registerInfo: RegisterRequest) {
    this.httpClient
      .post<Identity>(`${this.url}/${this.uri}/register`, registerInfo, {
        headers: this.header,
      })
      .subscribe({
        next: (x): Identity => {
          this.userSubject$.next({
            token: x.token,
            refreshToken: x.refreshToken,
          });

          localStorage.setItem('user', JSON.stringify(x));
          this.router.navigate(['/profile']);

          return x;
        },
        error: (e) => this.errorAlert(e),
      });
  }

  refresh() {
    const refreshRequest: RefreshRequest = {
      device: {
        id: 'b691b7a8-b251-4b11-8034-f3a0a154dffe',
        name: 'iPhone (John)',
      },
      token: this.currentUser.token,
      refreshToken: this.currentUser.refreshToken,
    };

    this.httpClient
      .post<Identity>(`${this.url}/${this.uri}/refresh`, refreshRequest)
      .subscribe((x): Identity => {
        this.userSubject$.next({
          token: x.token,
          refreshToken: x.refreshToken,
        });

        localStorage.setItem('user', JSON.stringify(x));

        return x;
      });
  }

  logout() {
    localStorage.removeItem('user');
    this.userSubject$.next(this.emptyUser);
    this.router.navigate(['/']);
  }

  errorAlert(e: any): void {
    let errorMessage = '';
    e.error.errors.forEach((errMsg: string) => {
      errorMessage += errMsg + ' ';
    });

    alert(errorMessage);
  }
}
