import { Component, Input, OnInit } from '@angular/core';
import { ActivatedRoute } from '@angular/router';
import { SystemContainers } from '../system-containers-list/system-containers-list.component';
import { DockerContainerSystem } from 'src/app/_models/docker-container-system';
import { environment } from 'src/environments/environment';
import { HttpClient } from '@angular/common/http';

@Component({
  selector: 'app-system-containers-detail',
  templateUrl: './system-containers-detail.component.html',
  styleUrls: ['./system-containers-detail.component.css']
})
export class SystemContainersDetailComponent implements OnInit {
  @Input() container: DockerContainerSystem | undefined
  baseUrl = environment.dockerapiUrl
  containerDetails: any
  test_image = "testcloudreg1/ocl_reg:dockerapi"

  constructor(private http: HttpClient, private systemContainers: SystemContainers, private route: ActivatedRoute) { }

  ngOnInit(): void {
    console.log('CONTAINER INIT')
    console.log(this.container)
    this.containerStats()
  }

  containerStats() {
    let split = this.container?.image.split(":", 2);
    this.http.post(this.baseUrl + 'docker/containers-stats', {
      RepositoryName: split![0],
      Tag: split![1]
    }).subscribe((data) => {
      this.containerDetails = data
    })
  }

}