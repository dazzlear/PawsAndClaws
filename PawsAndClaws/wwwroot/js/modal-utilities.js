/**
 * Modal Utilities
 * Common modal scenarios used throughout the application
 */
const ModalUtilities = {
  /**
   * Shows a success notification for common operations
   * @param {string} operation - Description of the operation (e.g., "Profile Updated", "Pet Added")
   * @param {function} callback - Optional callback after modal closes
   */
  success: function (operation, callback = null) {
    const messages = {
      "Profile Updated": "Your profile has been saved successfully.",
      "Pet Added": "The pet has been added to the inventory.",
      "Application Submitted":
        "Your application has been submitted for review.",
      "Application Updated": "The application status has been updated.",
      "Pet Deleted": "The pet has been removed from inventory.",
      "Signed Out": "You have been successfully signed out.",
    };

    const message =
      messages[operation] || "Your action has been completed successfully.";

    ModalManager.show(operation, message, "OKAY", callback);
  },

  /**
   * Shows an error notification
   * @param {string} title - Error title
   * @param {string} message - Error message
   * @param {function} callback - Optional callback after modal closes
   */
  error: function (title, message, callback = null) {
    ModalManager.show(title, message, "TRY AGAIN", callback);
  },

  /**
   * Shows a confirmation for destructive actions
   * @param {string} item - Item being deleted (e.g., "pet", "application")
   * @param {string} itemName - Name of the item
   * @param {function} onConfirm - Callback if user confirms
   */
  confirmDelete: function (item, itemName, onConfirm) {
    const messages = {
      pet: `Are you sure you want to remove ${itemName} from the inventory? This action cannot be undone.`,
      application: `Are you sure you want to remove this application? This action cannot be undone.`,
      profile: `Are you sure you want to delete your profile? All your data will be permanently removed.`,
    };

    const message =
      messages[item] || `Are you sure you want to delete ${itemName}?`;

    ModalManager.confirm(
      "Delete permanently?",
      message,
      "DELETE",
      "CANCEL",
      onConfirm,
      () => {
        // Do nothing if cancelled
      }
    );
  },

  /**
   * Shows a confirmation for leaving/unsaved changes
   * @param {function} onConfirm - Callback if user confirms leaving
   */
  confirmLeave: function (onConfirm) {
    ModalManager.confirm(
      "Unsaved changes",
      "You have unsaved changes. Are you sure you want to leave without saving?",
      "LEAVE",
      "KEEP EDITING",
      onConfirm,
      () => {
        // Do nothing if they choose to keep editing
      }
    );
  },

  /**
   * Shows a network error notification
   * @param {function} onRetry - Callback to retry the action
   */
  networkError: function (onRetry = null) {
    ModalManager.confirm(
      "Connection error",
      "We couldn't complete your request. Please check your internet connection and try again.",
      "RETRY",
      "CANCEL",
      onRetry,
      () => {
        // Do nothing if cancelled
      }
    );
  },

  /**
   * Shows a validation error
   * @param {string} field - Field that failed validation
   * @param {string} message - Validation message
   */
  validationError: function (field, message) {
    ModalManager.show(`${field} is invalid`, message, "GOT IT");
  },
};
