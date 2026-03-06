using Assessment.Domain.Entities;
using Microsoft.EntityFrameworkCore;

namespace Assessment.Infrastructure.Persistence
{
    public static class DbSeeder
    {
        public static async Task SeedAsync(ApplicationDbContext context)
        {
            // ------- Tech Stacks -------
            var techNames = new[]
            {
                "Angular","C#","CSS","HTML","JavaScript","Python","React","TypeScript"
            };

            foreach (var name in techNames)
            {
                var exists = await context.TechStacks.AnyAsync(x => x.Name == name);
                if (!exists)
                {
                    context.TechStacks.Add(new TechStack
                    {
                        Id = Guid.NewGuid(),
                        Name = name
                    });
                }
            }

            await context.SaveChangesAsync();

            var techMap = await context.TechStacks.AsNoTracking()
                .Where(x => techNames.Contains(x.Name))
                .ToDictionaryAsync(x => x.Name, x => x.Id);


            var existingQ = await context.Questions.CountAsync();
            if (existingQ > 0)
            {
                var testQuestions = await context.HrTestQuestions.ToListAsync();
                context.HrTestQuestions.RemoveRange(testQuestions);

                var answers = await context.UserAnswers.ToListAsync();
                context.UserAnswers.RemoveRange(answers);

                var options = await context.AnswerOptions.ToListAsync();
                context.AnswerOptions.RemoveRange(options);

                var questionsToDelete = await context.Questions.ToListAsync();
                context.Questions.RemoveRange(questionsToDelete);

                await context.SaveChangesAsync();
            }

            var questions = new List<Question>();
            int order = 1;

            void AddQuestion(string tech, QuestionLevel level, string text,
                string a, bool ca, string b, bool cb, string c, bool cc, string d, bool cd)
            {
                var qId = Guid.NewGuid();
                questions.Add(new Question
                {
                    Id = qId,
                    Order = order++,
                    TechStackId = techMap[tech],
                    Level = level,
                    Text = text,
                    TimeLimitSeconds = 60,
                    IsActive = true,
                    Options = new List<AnswerOption>
                    {
                        new AnswerOption { Id = Guid.NewGuid(), QuestionId = qId, Text = a, IsCorrect = ca },
                        new AnswerOption { Id = Guid.NewGuid(), QuestionId = qId, Text = b, IsCorrect = cb },
                        new AnswerOption { Id = Guid.NewGuid(), QuestionId = qId, Text = c, IsCorrect = cc },
                        new AnswerOption { Id = Guid.NewGuid(), QuestionId = qId, Text = d, IsCorrect = cd },
                    }
                });
            }

            AddQuestion("Angular", QuestionLevel.Beginner,
                "In Angular, which decorator defines a component?",
                "@NgModule", false, "@Component", true, "@Injectable", false, "@Pipe", false);

            AddQuestion("Angular", QuestionLevel.Beginner,
                "Which file typically defines routes in an Angular app?",
                "routes.ts / app-routing.module.ts", true, "styles.css", false, "polyfills.ts", false, "index.html", false);

            AddQuestion("Angular", QuestionLevel.Beginner,
                "Which directive repeats elements in a template?",
                "*ngIf", false, "*ngFor", true, "ngModel", false, "ngSwitch", false);

            AddQuestion("Angular", QuestionLevel.Beginner,
                "Which binding syntax is used for event binding?",
                "[prop]", false, "(event)", true, "{{ interpolation }}", false, "[(twoWay)]", false);

            AddQuestion("Angular", QuestionLevel.Beginner,
                "Which CLI command creates a new component?",
                "ng add component", false, "ng generate component", true, "ng build component", false, "ng serve component", false);

            AddQuestion("Angular", QuestionLevel.Intermediate,
                "Which RxJS operator is commonly used to switch to a new observable and cancel the previous one?",
                "map", false, "switchMap", true, "filter", false, "tap", false);

            AddQuestion("Angular", QuestionLevel.Intermediate,
                "What is a common place to provide singleton services application-wide?",
                "Component providers", false, "root injector (providedIn: 'root')", true, "template", false, "styles", false);

            AddQuestion("Angular", QuestionLevel.Intermediate,
                "Which Angular feature helps prevent loading modules until needed?",
                "Eager loading", false, "Lazy loading", true, "Change detection", false, "View encapsulation", false);

            AddQuestion("Angular", QuestionLevel.Intermediate,
                "Which lifecycle hook runs after Angular initializes component input properties?",
                "ngOnInit", true, "ngOnDestroy", false, "ngAfterViewInit", false, "ngDoCheck", false);

            AddQuestion("Angular", QuestionLevel.Intermediate,
                "What does 'AsyncPipe' primarily do?",
                "Formats numbers", false, "Subscribes/unsubscribes from observables automatically", true, "Validates forms", false, "Compiles templates", false);

            AddQuestion("Angular", QuestionLevel.Professional,
                "Best way to avoid memory leaks with subscriptions in components?",
                "Never subscribe", false, "Use takeUntil + Subject in ngOnDestroy", true, "Use console.log", false, "Disable change detection", false);

            AddQuestion("Angular", QuestionLevel.Professional,
                "Which strategy reduces change detection checks for a component?",
                "Default", false, "OnPush", true, "ManualOnly", false, "ZoneLessOnly", false);

            AddQuestion("Angular", QuestionLevel.Professional,
                "What does Angular route resolver do?",
                "Creates routes", false, "Fetches data before route activates", true, "Styles routes", false, "Lazy loads CSS", false);

            AddQuestion("Angular", QuestionLevel.Professional,
                "In large apps, recommended state management option?",
                "Direct DOM manipulation", false, "NgRx/Signals based patterns", true, "Inline scripts", false, "Hardcoded JSON", false);

            AddQuestion("Angular", QuestionLevel.Professional,
                "What is a key benefit of standalone components?",
                "More CSS", false, "Less module boilerplate", true, "Slower builds", false, "Removes routing", false);

            AddQuestion("C#", QuestionLevel.Beginner,
                "Which keyword defines a class in C#?",
                "function", false, "class", true, "def", false, "structOnly", false);

            AddQuestion("C#", QuestionLevel.Beginner,
                "What is the correct entry point method signature in a console app?",
                "public void Main()", false, "static void Main(string[] args)", true, "int main()", false, "Main(): void", false);

            AddQuestion("C#", QuestionLevel.Beginner,
                "Which type is used for whole numbers?",
                "string", false, "int", true, "bool", false, "DateTime", false);

            AddQuestion("C#", QuestionLevel.Beginner,
                "What does 'using' statement commonly manage?",
                "Loops", false, "Disposing resources (IDisposable)", true, "Inheritance", false, "Serialization", false);

            AddQuestion("C#", QuestionLevel.Beginner,
                "Which keyword prevents a class from being inherited?",
                "sealed", true, "static", false, "virtual", false, "partial", false);

            AddQuestion("C#", QuestionLevel.Intermediate,
                "Which collection guarantees unique items?",
                "List<T>", false, "HashSet<T>", true, "Queue<T>", false, "Stack<T>", false);

            AddQuestion("C#", QuestionLevel.Intermediate,
                "What is LINQ primarily used for?",
                "UI design", false, "Querying collections/data", true, "Multithreading only", false, "Networking only", false);

            AddQuestion("C#", QuestionLevel.Intermediate,
                "What does 'async/await' help with?",
                "CPU overclock", false, "Non-blocking async operations", true, "Encrypt strings", false, "Avoid exceptions", false);

            AddQuestion("C#", QuestionLevel.Intermediate,
                "Which access modifier makes a member visible only within its class?",
                "public", false, "private", true, "internal", false, "protected internal", false);

            AddQuestion("C#", QuestionLevel.Intermediate,
                "Which is true about 'var'?",
                "var is dynamic always", false, "var is compile-time inferred type", true, "var is object always", false, "var disables typing", false);

            AddQuestion("C#", QuestionLevel.Professional,
                "What does 'IQueryable' enable compared to 'IEnumerable' in EF Core?",
                "In-memory execution always", false, "Query translation to SQL", true, "Faster UI rendering", false, "No execution", false);

            AddQuestion("C#", QuestionLevel.Professional,
                "What is the best practice for HttpClient usage?",
                "new HttpClient per call", false, "Use IHttpClientFactory", true, "Use WebClient", false, "Use Thread.Sleep", false);

            AddQuestion("C#", QuestionLevel.Professional,
                "What does 'ConfigureAwait(false)' do?",
                "Blocks thread", false, "Avoids capturing synchronization context", true, "Adds caching", false, "Forces UI update", false);

            AddQuestion("C#", QuestionLevel.Professional,
                "Which pattern is recommended for large apps to separate layers?",
                "Spaghetti code", false, "Clean Architecture", true, "Inline SQL only", false, "No DTOs", false);

            AddQuestion("C#", QuestionLevel.Professional,
                "In EF Core, how to avoid N+1 when loading navigation properties?",
                "Disable tracking", false, "Use Include/ThenInclude", true, "Use random()", false, "Use AsParallel()", false);

            // ---------------- CSS (15) ----------------
            AddQuestion("CSS", QuestionLevel.Beginner,
                "Which property changes text color?",
                "font-style", false, "color", true, "background", false, "display", false);

            AddQuestion("CSS", QuestionLevel.Beginner,
                "Which selector targets an element by id?",
                ".class", false, "#id", true, "tag", false, "*", false);

            AddQuestion("CSS", QuestionLevel.Beginner,
                "Which property controls element spacing outside the border?",
                "padding", false, "margin", true, "border-radius", false, "gap", false);

            AddQuestion("CSS", QuestionLevel.Beginner,
                "Which value makes an element a flex container?",
                "display: grid", false, "display: flex", true, "position: flex", false, "flex: display", false);

            AddQuestion("CSS", QuestionLevel.Beginner,
                "Which property changes background color?",
                "background-color", true, "text-color", false, "font-color", false, "color-bg", false);

            AddQuestion("CSS", QuestionLevel.Intermediate,
                "Which unit is relative to the root font-size?",
                "em", false, "rem", true, "px", false, "cm", false);

            AddQuestion("CSS", QuestionLevel.Intermediate,
                "What does 'box-sizing: border-box' do?",
                "Adds shadow", false, "Includes padding/border in width/height", true, "Centers element", false, "Makes text bold", false);

            AddQuestion("CSS", QuestionLevel.Intermediate,
                "Which property creates space between flex items?",
                "gap", true, "padding", false, "border", false, "outline", false);

            AddQuestion("CSS", QuestionLevel.Intermediate,
                "Which pseudo-class targets element on hover?",
                ":active", false, ":hover", true, ":focus", false, ":visited", false);

            AddQuestion("CSS", QuestionLevel.Intermediate,
                "Which layout is best for 2D grid systems?",
                "Flexbox", false, "CSS Grid", true, "Float", false, "Inline-block", false);

            AddQuestion("CSS", QuestionLevel.Professional,
                "What is specificity order (higher priority)?",
                "tag < class < id", true, "id < class < tag", false, "class < tag < id", false, "all equal", false);

            AddQuestion("CSS", QuestionLevel.Professional,
                "Which is best for responsive typography?",
                "fixed px everywhere", false, "clamp()", true, "only pt", false, "hard-coded rem only", false);

            AddQuestion("CSS", QuestionLevel.Professional,
                "What does 'position: sticky' require to work properly?",
                "display: flex", false, "a top/left/right/bottom offset", true, "box-shadow", false, "z-index: 0", false);

            AddQuestion("CSS", QuestionLevel.Professional,
                "Which method avoids layout shift for images?",
                "no width/height", false, "set width/height or aspect-ratio", true, "use border", false, "use float", false);

            AddQuestion("CSS", QuestionLevel.Professional,
                "What is a common performance improvement in animations?",
                "animate width/height", false, "animate transform/opacity", true, "animate border", false, "animate left/top", false);

            // ---------------- HTML (15) ----------------
            AddQuestion("HTML", QuestionLevel.Beginner,
                "What does HTML stand for?",
                "HyperText Markup Language", true, "HighText Markdown Language", false, "Hyper Tool Multi Language", false, "None", false);

            AddQuestion("HTML", QuestionLevel.Beginner,
                "Which tag creates a hyperlink?",
                "<link>", false, "<a>", true, "<href>", false, "<url>", false);

            AddQuestion("HTML", QuestionLevel.Beginner,
                "Which tag is used for an image?",
                "<img>", true, "<image>", false, "<pic>", false, "<src>", false);

            AddQuestion("HTML", QuestionLevel.Beginner,
                "Which tag is used for a form input?",
                "<input>", true, "<field>", false, "<text>", false, "<forminput>", false);

            AddQuestion("HTML", QuestionLevel.Beginner,
                "Which semantic tag represents navigation links?",
                "<nav>", true, "<section>", false, "<div>", false, "<header>", false);

            AddQuestion("HTML", QuestionLevel.Intermediate,
                "Which attribute links a label to an input?",
                "id", false, "for", true, "name", false, "data-label", false);

            AddQuestion("HTML", QuestionLevel.Intermediate,
                "What does the 'required' attribute do?",
                "Adds styles", false, "Prevents submit if empty", true, "Encrypts input", false, "Adds placeholder", false);

            AddQuestion("HTML", QuestionLevel.Intermediate,
                "Which input type is used for email validation?",
                "text", false, "email", true, "mail", false, "validate", false);

            AddQuestion("HTML", QuestionLevel.Intermediate,
                "Which tag is used for table row?",
                "<tr>", true, "<td>", false, "<th>", false, "<row>", false);

            AddQuestion("HTML", QuestionLevel.Intermediate,
                "Which tag is used to group form elements with a caption?",
                "<fieldset>", true, "<legendary>", false, "<group>", false, "<section>", false);

            AddQuestion("HTML", QuestionLevel.Professional,
                "What is the purpose of ARIA attributes?",
                "Improve CSS", false, "Improve accessibility for assistive tech", true, "Speed up JS", false, "Replace HTML", false);

            AddQuestion("HTML", QuestionLevel.Professional,
                "Which meta tag is important for responsive design?",
                "charset", false, "viewport", true, "author", false, "robots", false);

            AddQuestion("HTML", QuestionLevel.Professional,
                "What does 'defer' do on a script tag?",
                "Runs immediately", false, "Executes after HTML parsing", true, "Blocks rendering", false, "Disables JS", false);

            AddQuestion("HTML", QuestionLevel.Professional,
                "What is a correct use of <button type=\"submit\">?",
                "Inside a form to submit", true, "Only outside forms", false, "Only with href", false, "Only with image", false);

            AddQuestion("HTML", QuestionLevel.Professional,
                "Which attribute is used to open link in new tab securely?",
                "target=_blank only", false, "target=_blank + rel=\"noopener noreferrer\"", true, "rel=css", false, "noopener is invalid", false);

            // ---------------- JavaScript (15) ----------------
            AddQuestion("JavaScript", QuestionLevel.Beginner,
                "Which keyword declares a block-scoped variable?",
                "var", false, "let", true, "define", false, "dim", false);

            AddQuestion("JavaScript", QuestionLevel.Beginner,
                "Which method converts JSON string to object?",
                "JSON.stringify", false, "JSON.parse", true, "JSON.toObject", false, "parse.JSON", false);

            AddQuestion("JavaScript", QuestionLevel.Beginner,
                "What is the result type of 'typeof []'?",
                "array", false, "object", true, "list", false, "null", false);

            AddQuestion("JavaScript", QuestionLevel.Beginner,
                "Which symbol is used for strict equality?",
                "==", false, "===", true, "!=", false, "=", false);

            AddQuestion("JavaScript", QuestionLevel.Beginner,
                "Which function runs after a delay?",
                "setTimeout", true, "setNow", false, "delay()", false, "sleep()", false);

            AddQuestion("JavaScript", QuestionLevel.Intermediate,
                "Which array method creates a new array by transforming items?",
                "forEach", false, "map", true, "push", false, "splice", false);

            AddQuestion("JavaScript", QuestionLevel.Intermediate,
                "What does 'Promise.all' do?",
                "Runs promises sequentially", false, "Resolves when all resolve, rejects if any rejects", true, "Cancels promises", false, "Delays execution", false);

            AddQuestion("JavaScript", QuestionLevel.Intermediate,
                "What is event bubbling?",
                "Event stops at target", false, "Event propagates from child to parent", true, "Event goes only to window", false, "Event only on capture", false);

            AddQuestion("JavaScript", QuestionLevel.Intermediate,
                "What is a closure?",
                "A CSS feature", false, "Function + preserved lexical scope", true, "A loop", false, "A DOM node", false);

            AddQuestion("JavaScript", QuestionLevel.Intermediate,
                "Which keyword prevents reassignment of a variable binding?",
                "var", false, "const", true, "let", false, "static", false);

            AddQuestion("JavaScript", QuestionLevel.Professional,
                "What does the event loop manage?",
                "GPU rendering", false, "Async callbacks and task queues", true, "CSS parsing", false, "HTTP encryption", false);

            AddQuestion("JavaScript", QuestionLevel.Professional,
                "What is a common cause of memory leaks?",
                "Too many console.logs", false, "Unremoved event listeners / retained references", true, "Using let", false, "Using strict mode", false);

            AddQuestion("JavaScript", QuestionLevel.Professional,
                "Which is best for deep cloning in modern JS (with limitations)?",
                "Object.assign only", false, "structuredClone()", true, "JSON.parse(JSON.stringify()) always best", false, "clone()", false);

            AddQuestion("JavaScript", QuestionLevel.Professional,
                "What is tree-shaking?",
                "Runtime bundling", false, "Removing unused exports in bundling", true, "Minifying HTML only", false, "Caching API responses", false);

            AddQuestion("JavaScript", QuestionLevel.Professional,
                "Which is correct about module scope in ES modules?",
                "Everything is global", false, "Top-level is module-scoped", true, "Same as script tags always", false, "No imports allowed", false);

            // ---------------- Python (15) ----------------
            AddQuestion("Python", QuestionLevel.Beginner,
                "Which keyword defines a function in Python?",
                "func", false, "def", true, "function", false, "lambda", false);

            AddQuestion("Python", QuestionLevel.Beginner,
                "Which data type is immutable?",
                "list", false, "tuple", true, "dict", false, "set", false);

            AddQuestion("Python", QuestionLevel.Beginner,
                "How do you start a comment line?",
                "//", false, "#", true, "/*", false, "<!--", false);

            AddQuestion("Python", QuestionLevel.Beginner,
                "Which function prints output?",
                "echo()", false, "print()", true, "console.log()", false, "out()", false);

            AddQuestion("Python", QuestionLevel.Beginner,
                "Which operator is used for exponentiation?",
                "^", false, "**", true, "*", false, "//", false);

            AddQuestion("Python", QuestionLevel.Intermediate,
                "What does list comprehension produce?",
                "A generator always", false, "A list", true, "A dict", false, "A tuple", false);

            AddQuestion("Python", QuestionLevel.Intermediate,
                "Which keyword handles exceptions?",
                "catch", false, "except", true, "rescue", false, "error", false);

            AddQuestion("Python", QuestionLevel.Intermediate,
                "What does 'pip' primarily do?",
                "Runs scripts", false, "Installs packages", true, "Formats code", false, "Compiles Python", false);

            AddQuestion("Python", QuestionLevel.Intermediate,
                "Which statement opens a file safely?",
                "open()", false, "with open(...) as f", true, "file.open()", false, "readfile()", false);

            AddQuestion("Python", QuestionLevel.Intermediate,
                "What does 'None' represent?",
                "zero", false, "absence of value", true, "empty string", false, "false always", false);

            AddQuestion("Python", QuestionLevel.Professional,
                "What is a virtual environment used for?",
                "Speed up CPU", false, "Isolate dependencies per project", true, "Encrypt code", false, "Compile to C", false);

            AddQuestion("Python", QuestionLevel.Professional,
                "What does GIL affect in CPython?",
                "Disk IO", false, "True parallel CPU threads", true, "HTTP requests", false, "JSON parsing", false);

            AddQuestion("Python", QuestionLevel.Professional,
                "What is a generator?",
                "Always a list", false, "Produces values lazily using yield", true, "A dict type", false, "A class decorator", false);

            AddQuestion("Python", QuestionLevel.Professional,
                "Which improves type checking in Python projects?",
                "mypy/pyright", true, "pip only", false, "numpy", false, "pytest only", false);

            AddQuestion("Python", QuestionLevel.Professional,
                "Which is best for structured data validation in Python APIs?",
                "string concatenation", false, "Pydantic", true, "print()", false, "random()", false);

            // ---------------- React (15) ----------------
            AddQuestion("React", QuestionLevel.Beginner,
                "What does JSX compile to?",
                "SQL", false, "JavaScript function calls", true, "CSS", false, "HTML only", false);

            AddQuestion("React", QuestionLevel.Beginner,
                "Which hook manages component state?",
                "useEffect", false, "useState", true, "useMemo", false, "useRef", false);

            AddQuestion("React", QuestionLevel.Beginner,
                "Which prop is required for lists?",
                "index", false, "key", true, "id", false, "name", false);

            AddQuestion("React", QuestionLevel.Beginner,
                "How do you pass data from parent to child?",
                "props", true, "state only", false, "context only", false, "router", false);

            AddQuestion("React", QuestionLevel.Beginner,
                "Which hook runs side effects?",
                "useState", false, "useEffect", true, "useCallback", false, "useId", false);

            AddQuestion("React", QuestionLevel.Intermediate,
                "What does useMemo do?",
                "Always rerenders", false, "Memoizes computed value", true, "Manages routes", false, "Fetches data", false);

            AddQuestion("React", QuestionLevel.Intermediate,
                "What does useCallback do?",
                "Memoizes a function reference", true, "Stores DOM", false, "Creates state", false, "Stops renders", false);

            AddQuestion("React", QuestionLevel.Intermediate,
                "Which is correct about controlled components?",
                "Input manages itself", false, "Value comes from state", true, "Must use refs", false, "Only for forms", false);

            AddQuestion("React", QuestionLevel.Intermediate,
                "What is lifting state up?",
                "Moving state to child", false, "Moving state to common parent", true, "Using context", false, "Using redux", false);

            AddQuestion("React", QuestionLevel.Intermediate,
                "Which API is used for navigation in React Router v6?",
                "useHistory", false, "useNavigate", true, "history.pushState", false, "navigateTo", false);

            AddQuestion("React", QuestionLevel.Professional,
                "What does React StrictMode do in dev?",
                "Removes warnings", false, "Double-invokes some lifecycles/effects to find issues", true, "Speeds production", false, "Disables hooks", false);

            AddQuestion("React", QuestionLevel.Professional,
                "What is a common cause of infinite re-render?",
                "Using CSS", false, "Calling setState in render without condition", true, "Using useRef", false, "Using memo", false);

            AddQuestion("React", QuestionLevel.Professional,
                "Best practice for expensive list rendering?",
                "Render all always", false, "Virtualization (react-window)", true, "Use alerts", false, "Use tables only", false);

            AddQuestion("React", QuestionLevel.Professional,
                "What is the recommended way to fetch data in effects?",
                "Directly in render", false, "Inside useEffect with cleanup/cancellation", true, "Only in constructor", false, "Only in reducers", false);

            AddQuestion("React", QuestionLevel.Professional,
                "Why use React Query / SWR?",
                "For CSS themes", false, "Caching + retries + deduping + invalidation", true, "For routing", false, "To remove hooks", false);

            // ---------------- TypeScript (15) ----------------
            AddQuestion("TypeScript", QuestionLevel.Beginner,
                "What does TypeScript add to JavaScript?",
                "CSS", false, "Static typing", true, "SQL", false, "DOM only", false);

            AddQuestion("TypeScript", QuestionLevel.Beginner,
                "Which type represents a true/false value?",
                "string", false, "boolean", true, "number", false, "any", false);

            AddQuestion("TypeScript", QuestionLevel.Beginner,
                "How do you define an array of numbers?",
                "number[]", true, "int[]", false, "array<number>", false, "numbers()", false);

            AddQuestion("TypeScript", QuestionLevel.Beginner,
                "What does 'any' mean?",
                "Strictly typed", false, "Opt-out of type checking", true, "Only numbers", false, "Only strings", false);

            AddQuestion("TypeScript", QuestionLevel.Beginner,
                "Which keyword defines an interface?",
                "type", false, "interface", true, "class", false, "struct", false);

            AddQuestion("TypeScript", QuestionLevel.Intermediate,
                "What is a union type?",
                "One fixed type", false, "A type that can be one of several", true, "A class", false, "A namespace", false);

            AddQuestion("TypeScript", QuestionLevel.Intermediate,
                "What does 'unknown' require before usage?",
                "Nothing", false, "Type narrowing/checks", true, "Casting to any", false, "Compilation flag", false);

            AddQuestion("TypeScript", QuestionLevel.Intermediate,
                "What is generics used for?",
                "Hardcoding types", false, "Type-safe reusable components/functions", true, "Only enums", false, "Only interfaces", false);

            AddQuestion("TypeScript", QuestionLevel.Intermediate,
                "Which helps ensure a string is one of known values?",
                "enum / string literal union", true, "any", false, "unknown", false, "void", false);

            AddQuestion("TypeScript", QuestionLevel.Intermediate,
                "What does 'as const' do?",
                "Makes value mutable", false, "Narrows literals to readonly literal types", true, "Disables TS", false, "Adds runtime checks", false);

            AddQuestion("TypeScript", QuestionLevel.Professional,
                "What does conditional type allow?",
                "Runtime if", false, "Type-level branching", true, "CSS conditions", false, "SQL joins", false);

            AddQuestion("TypeScript", QuestionLevel.Professional,
                "Why prefer 'unknown' over 'any'?",
                "unknown is faster", false, "unknown forces type safety", true, "any is safer", false, "unknown removes types", false);

            AddQuestion("TypeScript", QuestionLevel.Professional,
                "What is utility type 'Partial<T>'?",
                "Makes properties required", false, "Makes all properties optional", true, "Removes properties", false, "Converts to array", false);

            AddQuestion("TypeScript", QuestionLevel.Professional,
                "How to type an API response generically?",
                "Use var", false, "Use generics like ApiResponse<T>", true, "Use only any", false, "No typing", false);

            AddQuestion("TypeScript", QuestionLevel.Professional,
                "Which is best to model DTOs in frontend?",
                "string-only", false, "interfaces/types + runtime validation if needed", true, "no types", false, "inline JSON only", false);

            context.Questions.AddRange(questions);
            await context.SaveChangesAsync();
        }
    }
}