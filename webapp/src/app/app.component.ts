import { Component, OnInit } from '@angular/core';
import { User } from './_models/user';
import { AuthService } from './_services/auth.service';

@Component({
  selector: 'app-root',
  templateUrl: './app.component.html',
  styleUrls: ['./app.component.css']
})
export class AppComponent implements OnInit {
  title = 'Docker';
  users: any;

  constructor(private authService: AuthService) { }

  ngOnInit() {
    this.setCurrentUser()
  }

  setCurrentUser() {
    const user: User = JSON.parse(localStorage.getItem('user')!)
    this.authService.setCurrentUser(user)
  }
}
