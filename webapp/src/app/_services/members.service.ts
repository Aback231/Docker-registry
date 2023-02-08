import { Injectable } from '@angular/core';
import { environment } from 'src/environments/environment';
import { HttpClient, HttpHeaders } from '@angular/common/http';
import { Member } from '../_models/member';

@Injectable({
  providedIn: 'root'
})
export class MembersService {
  baseUrl = environment.webapiUrl;

  constructor(private http: HttpClient) { }

  getMembers() {
    return this.http.get<Member[]>(this.baseUrl + 'users');
  }

  getMember(id: string) {
    return this.http.get<Member>(this.baseUrl + 'users/' + id);
  }

  getContainers() {
    return this.http.get<Member[]>(this.baseUrl + 'docker/user-containers');
  }
}