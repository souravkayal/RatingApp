import { Component } from '@angular/core';
import { HttpClient } from '@angular/common/http';



@Component({
  selector: 'app-home',
  templateUrl: './home.component.html',
})
export class HomeComponent {

  product: any;

  constructor(http: HttpClient) {

    http.post('https://localhost:44354/api/rating/readRating', [1, 2])
      .subscribe(result => {

        this.product = result;

        console.log(result)

    }, error => console.error(error));

  }

}
