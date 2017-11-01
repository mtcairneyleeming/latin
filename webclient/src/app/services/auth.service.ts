import {Injectable} from '@angular/core';
import {Router} from '@angular/router';

import Auth0 from 'auth0-js';

@Injectable()
export class AuthService {
  auth0 = new Auth0.WebAuth({
    clientID: '8Q62uKJ4m4yFU5LjpGhvTcOvRnFleNa1',
    domain: 'mcl.eu.auth0.com',
    responseType: 'token id_token',
    audience: 'latin-learning.mtcairneyleeming',
    redirectUri: 'http://localhost:4200/callback',
    scope: 'openid profile learn:all'
  });
  public userProfile: any;

  constructor(public router: Router) {
  }

  private static setSession(authResult): void {
    // Set the time that the access token will expire at
    const expiresAt = JSON.stringify((authResult.expiresIn * 1000) + new Date().getTime());
    localStorage.setItem('access_token', authResult.accessToken);
    localStorage.setItem('id_token', authResult.idToken);
    localStorage.setItem('expires_at', expiresAt);
  }

  public login(): void {
    this.auth0.authorize();
  }

  public handleAuthentication(): void {
    this.auth0.parseHash((err, authResult) => {
      if (authResult && authResult.accessToken && authResult.idToken) {
        window.location.hash = '';
        AuthService.setSession(authResult);
        this.getProfile(() => {
          return null;
        });
        this.router.navigate(['/home']);
      } else if (err) {
        this.router.navigate(['/home']);
        console.log(err);
      }
    });
  }

  public logout(): void {
    // Remove tokens and expiry time from localStorage
    localStorage.removeItem('access_token');
    localStorage.removeItem('id_token');
    localStorage.removeItem('expires_at');
    this.userProfile = null;
    // Go back to the home route
    this.router.navigate(['/']);
  }

  // noinspection JSMethodCanBeStatic // the auth service is already DI'd, so there's no need for that
  public isAuthenticated(): boolean {
    // Check whether the current time is past the
    // access token's expiry time
    const expiresAt = JSON.parse(localStorage.getItem('expires_at'));
    return new Date().getTime() < expiresAt;
  }

  public getProfile(cb): void {
    const accessToken = localStorage.getItem('access_token');
    if (!accessToken) {
      throw new Error('Access token must exist to fetch profile');
    }

    const self = this;
    this.auth0.client.userInfo(accessToken, (err, profile) => {
      if (profile) {
        self.userProfile = profile;
      }
      cb(err, profile);
    });
  }
}
