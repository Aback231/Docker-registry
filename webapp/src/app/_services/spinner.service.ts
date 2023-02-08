import { Injectable } from '@angular/core';
import { NgxSpinnerService } from 'ngx-spinner';

@Injectable({
  providedIn: 'root'
})

export class SpinnerService {
  reqCount = 0;

  constructor(private spinnerService: NgxSpinnerService) { }

  spin(){
    this.reqCount++
    this.spinnerService.show(undefined, {
      type: 'square-jelly-box',
      bdColor: 'rgba(0, 0, 0, 0.8)',
      color: '#fff',
      size: "medium",
    })
  }

  idle(){
    this.reqCount--
    if(this.reqCount <= 0){
      this.reqCount = 0
      this.spinnerService.hide()
    }
  }
}
