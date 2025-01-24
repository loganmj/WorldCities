import { Component } from '@angular/core';
import { AbstractControl, FormGroup } from '@angular/forms';

/**
 * A base class for all of our form components.
 */ 
@Component({
  template: ''
})
export class BaseFormComponent {

  // #region Properties

  /**
   * The form model.
   */ 
  public form!: FormGroup;

  // #endregion

  // #region Constructors
  // #endregion

  // #region Private Methods
  // #endregion

  // #region Public Methods

  /**
   * Parses errors and return appropriate error messages.
   * @param control
   * @param displayName 
   */
  public getErrors(control: AbstractControl, displayName: string): string[] {

    var errors: string[] = [];

    // Iterate over the errors in the control, and parse them into error message strings
    Object.keys(control.errors || {}).forEach((key) => {
      switch (key) {
        case 'required':
          errors.push(`${displayName} is required.`);
          break;
        case 'pattern':
          errors.push(`${displayName} contains invalid characters.`);
          break;
        case 'isDuplicateField':
          errors.push(`${displayName} already exists: please choose another.`);
          break;
        default:
          errors.push(`${displayName} is invalid.`);
          break;
      }
    });

    return errors;
  }

  // #endregion
}
