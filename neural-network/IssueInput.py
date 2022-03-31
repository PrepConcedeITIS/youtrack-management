class IssueInput:
    def __init__(self, assignee_login: str, complexity: str, issue_type: str, tags_concatenated: list[str], id: str):
        self.AssigneeLogin = assignee_login
        self.Complexity = complexity
        self.IssueType = issue_type
        self.TagsConcatenated = tags_concatenated
        self.Id = id
