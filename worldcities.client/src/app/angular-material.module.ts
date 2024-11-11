import { NgModule } from "@angular/core";
import { MatButtonModule } from "@angular/material/button";
import { MatIconModule } from "@angular/material/icon";
import { MatToolbarModule } from "@angular/material/toolbar";
import { MatTableModule } from '@angular/material/table';
import { MatPaginator } from "@angular/material/paginator";

@NgModule({

  // Lists modules that are being imported by this module
  imports: [
    MatButtonModule,
    MatIconModule,
    MatToolbarModule,
    MatTableModule,
    MatPaginator
  ],

  // Lists modules that will be exported with this module, so they don't have to
  // be imported again by consuming modules.
  exports: [
    MatButtonModule,
    MatIconModule,
    MatToolbarModule,
    MatTableModule,
    MatPaginator
  ]
})

export class AngularMaterialModule { }
