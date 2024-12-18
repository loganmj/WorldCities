import { HttpClient } from '@angular/common/http';
import { Component, OnInit } from '@angular/core';

@Component({
  selector: 'app-root',
  templateUrl: './app.component.html',
  styleUrl: './app.component.scss',
  standalone: false
})
export class AppComponent implements OnInit {

  constructor(private http: HttpClient) { }

  ngOnInit() {
    // TODO
  }

  title = 'worldcities2.client';
}
