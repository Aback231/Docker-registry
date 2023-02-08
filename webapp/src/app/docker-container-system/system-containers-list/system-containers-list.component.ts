import { Component, OnInit } from '@angular/core';
import { DockerContainerSystem } from 'src/app/_models/docker-container-system';
import { DockersService } from 'src/app/_services/dockers.service';

@Component({
  selector: 'app-system-containers',
  templateUrl: './system-containers-list.component.html',
  styleUrls: ['./system-containers-list.component.css']
})
export class SystemContainers implements OnInit {
  containers: DockerContainerSystem[] = []
  route: any;

  constructor(private dockersService: DockersService) { }

  ngOnInit(): void {
    this.loadContainers();
  }

  loadContainers() {
    this.dockersService.getContainers().subscribe(containers => {
      this.containers = containers;
      console.log(this.containers[0])
    })
  }
}