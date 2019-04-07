import { Component, OnInit, OnDestroy } from '@angular/core';
import { HttpClient } from '@angular/common/http';
import { Subscription } from 'rxjs';

const API_DOMAIN_URL = `http://localhost:5000`;
const VALUES_API_PATH = `/api/values`;

@Component({
  selector: 'app-value',
  templateUrl: './value.component.html',
  styleUrls: ['./value.component.css']
})
export class ValueComponent implements OnInit, OnDestroy {
  values: any;
  subscription: Subscription;

  constructor(private http:HttpClient) { }

  ngOnInit() {
    this.getValues();
  }

  ngOnDestroy(): void {
    this.subscription.unsubscribe();
  }

  getValues() {
    this.subscription = this.http.get(`${API_DOMAIN_URL}${VALUES_API_PATH}`)
      .subscribe(response => {
        this.values = response;
        console.log(`[DEBUG] - <Values> values: \n`, this.values);
      }, err => {
        console.log(`[ERROR] - <Values> Details: \n`, err);
      });
  }

}
