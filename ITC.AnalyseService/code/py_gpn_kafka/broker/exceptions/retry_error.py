class RetryConnectionError(Exception):
    def __init__(
        self, failed_operation_name: str, minimum_time_s: float, current_time_s: float, collected_error: Exception
    ):
        super(RetryConnectionError).__init__()
        self.message = failed_operation_name
        self.minimum_time_s = minimum_time_s
        self.current_time_s = current_time_s
        self.collected_error = collected_error

    def __str__(self):
        return (
            f"Failed to {self.message} due to '{type(self.collected_error).__name__}': {self.collected_error}; "
            f"Stop retrying due to "
            f"current retry interval - {self.current_time_s}s is less then minimum - {self.minimum_time_s}s"
        )
