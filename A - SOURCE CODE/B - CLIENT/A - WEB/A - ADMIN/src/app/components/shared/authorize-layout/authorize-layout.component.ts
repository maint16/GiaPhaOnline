import {Component, OnInit} from "@angular/core";

@Component({
  selector: 'authorize-layout',
  templateUrl: 'authorize-layout.component.html'
})

export class AuthorizeLayoutComponent implements OnInit{

  //#region Properties

  /*
  * Url parameter.
  * */
  private url: any;

  //#endregion

  //#region Constructor

  /*
  * Initiate component with injectors.
  * */
  public constructor(){
  }

  //#endregion

  //#region Methods

  /*
  * Event which is called when component has been initiated.
  * */
  public ngOnInit(): void {
  }

  //#endregion
}
