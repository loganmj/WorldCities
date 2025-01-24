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
   * Allows for passing in custom error messages.
   * @param control
   * @param displayName
   * @param [customMessages=null] 
   */
  public getErrors(control: AbstractControl, displayName: string, customMessages: {[key: string] : string} | null = null): string[] {

    var errors: string[] = [];

    // Iterate over the errors in the control, and parse them into error message strings
    Object.keys(control.errors || {}).forEach((key) => {
      switch (key) {
        case 'required':
          errors.push(`${displayName} ${customMessages?.[key] ?? "is required"}.`);
          break;
        case 'pattern':
          errors.push(`${displayName} ${customMessages?.[key] ?? "contains invalid characters"}.`);
          break;
        case 'isDuplicateField':
          errors.push(`${displayName} ${customMessages?.[key] ?? "already exists: please choose another"}.`);
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
