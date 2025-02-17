/**
 * An interface for a user registration result object.
 */ 
export interface RegisterResult {
  success: boolean;
  message: string;
  errors: string[];
}
