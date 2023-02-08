import { HttpClient } from '@angular/common/http';
import { Injectable } from '@angular/core';
import { ReplaySubject } from 'rxjs';
import { map } from 'rxjs/operators';
import { environment } from 'src/environments/environment';
import { User } from '../_models/user';

@Injectable({
  providedIn: 'root'
})
export class AuthService {
  baseUrl = environment.webapiUrl
  // Store current User object
  private currentUserSource = new ReplaySubject<User>(1)
  currentUser$ = this.currentUserSource.asObservable()

  constructor(private http: HttpClient) { }

  register(model: User) {
    return this.http.post(this.baseUrl + 'register', model).pipe(
      map((user: any) => {
        if (user) {
          localStorage.setItem('user', JSON.stringify(user))
          this.currentUserSource.next(user)
        }
      })
    )
  }

  login(model: User) {
    return this.http.post(this.baseUrl + 'login', model).pipe(
      map((response: any) => {
        const user = response
        if (user) {
          localStorage.setItem('user', JSON.stringify(user))
          this.currentUserSource.next(user)
        }
      })
    )
  }

  setCurrentUser(user: User) {
    this.currentUserSource.next(user)
  }

  logout() {
    localStorage.removeItem('user')
    this.currentUserSource.next(null!)
  }
}
