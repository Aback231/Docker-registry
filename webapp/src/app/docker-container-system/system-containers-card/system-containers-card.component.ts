import { HttpClient } from '@angular/common/http';
import { Component, OnInit, Input } from '@angular/core';
import { DockerContainerSystem } from 'src/app/_models/docker-container-system';
import { environment } from 'src/environments/environment';

@Component({
  selector: 'app-system-containers-card',
  templateUrl: './system-containers-card.component.html',
  styleUrls: ['./system-containers-card.component.css'],
})
export class SystemContainersCardComponent implements OnInit {

  @Input() container: DockerContainerSystem | undefined;
  containerDetails: any
  baseUrl = environment.dockerapiUrl
  serverAddres = environment.dockerHubServer
  username = environment.dockerHubUsername
  password = environment.dockerHubPassword
  test_image = "testcloudreg1/ocl_reg:dockerapi"

  pullImage(auto_update: boolean = false) {
    let split = this.container?.image.split(":", 2);
    this.http.post(this.baseUrl + 'docker/image-pull', {
      RepositoryName: split![0],
      Tag: split![1],
      ServerAddress: this.serverAddres,
      Username: this.username,
      Password: this.password
    }).subscribe((data) => {
      console.log(data)
      if(auto_update){
        this.containerUpdate()
      }
    })
  }

  containerUpdate() {
    let split = this.container?.image.split(":", 2);

    this.http.post(this.baseUrl + 'docker/container-update', {
      RepositoryName: split![0],
      Tag: split![1]
    }).subscribe((data) => {
      console.log(data)
    })
  }

  containerStop() {
    let split = this.container?.image.split(":", 2);
    this.http.post(this.baseUrl + 'docker/containers-stop', {
      RepositoryName: split![0],
      Tag: split![1]
    }).subscribe((data) => {
      console.log(data)
    })
  }

  // containerStart(local_update: boolean = false) {
  //   let split = this.container?.image.split(":", 2);
  //   this.http.post(this.baseUrl + 'docker/container-start', {
  //     RepositoryName: split![0],
  //     Tag: split![1]
  //   }).subscribe((data) => {
  //     console.log(data)
  //   })
  // }

  constructor(private http: HttpClient) {  }

  ngOnInit(): void {
  }

}