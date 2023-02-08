import { Injectable } from '@angular/core';
import { environment } from 'src/environments/environment';
import { HttpClient, HttpHeaders } from '@angular/common/http';
import { DockerContainerSystem } from '../_models/docker-container-system';

@Injectable({
  providedIn: 'root'
})
export class DockersService {
  baseUrl = environment.dockerapiUrl;

  constructor(private http: HttpClient) { }

  getContainers() {
    return this.http.get<DockerContainerSystem[]>(this.baseUrl + 'docker/list-containers');
  }

  // getImages() {
  //   return this.http.get<DockerContainerSystem>(this.baseUrl + 'docker/list-images');
  // }
}