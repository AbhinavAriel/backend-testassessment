using Assessment.Domain.Entities;
using Microsoft.EntityFrameworkCore;

namespace Assessment.Infrastructure.Persistence
{
    public static class DbSeeder
    {
        public static async Task SeedAsync(ApplicationDbContext context)
        {
            var techNames = new[]
            {
                "Angular", "C#", "CSS", "HTML", "JavaScript", "Python", "React", "TypeScript"
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

            int order = await context.Questions.AnyAsync()
                ? await context.Questions.MaxAsync(q => q.Order) + 1
                : 1;

            async Task AddQuestionIfMissing(
                string tech,
                QuestionLevel level,
                string text,
                string a, bool ca,
                string b, bool cb,
                string c, bool cc,
                string d, bool cd)
            {
                var techStackId = techMap[tech];

                var alreadyExists = await context.Questions
                    .AsNoTracking()
                    .AnyAsync(q =>
                        q.TechStackId == techStackId &&
                        q.Level == level &&
                        q.Text == text);

                if (alreadyExists)
                    return;

                var qId = Guid.NewGuid();

                context.Questions.Add(new Question
                {
                    Id = qId,
                    Order = order++,
                    TechStackId = techStackId,
                    Level = level,
                    Text = text,
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

            await AddQuestionIfMissing("Angular", QuestionLevel.Beginner,
                "In Angular, which decorator defines a component?",
                "@NgModule", false, "@Component", true, "@Injectable", false, "@Pipe", false);

            await AddQuestionIfMissing("Angular", QuestionLevel.Beginner,
                "Which file typically defines routes in an Angular app?",
                "routes.ts / app-routing.module.ts", true, "styles.css", false, "polyfills.ts", false, "index.html", false);

            await AddQuestionIfMissing("Angular", QuestionLevel.Beginner,
                "Which directive repeats elements in a template?",
                "*ngIf", false, "*ngFor", true, "ngModel", false, "ngSwitch", false);

            await AddQuestionIfMissing("Angular", QuestionLevel.Beginner,
                "Which binding syntax is used for event binding?",
                "[prop]", false, "(event)", true, "{{ interpolation }}", false, "[(twoWay)]", false);

            await AddQuestionIfMissing("Angular", QuestionLevel.Beginner,
                "Which CLI command creates a new component?",
                "ng add component", false, "ng generate component", true, "ng build component", false, "ng serve component", false);

            await AddQuestionIfMissing("Angular", QuestionLevel.Intermediate,
                "Which RxJS operator is commonly used to switch to a new observable and cancel the previous one?",
                "map", false, "switchMap", true, "filter", false, "tap", false);

            await AddQuestionIfMissing("Angular", QuestionLevel.Intermediate,
                "What is a common place to provide singleton services application-wide?",
                "Component providers", false, "root injector (providedIn: 'root')", true, "template", false, "styles", false);

            await AddQuestionIfMissing("Angular", QuestionLevel.Intermediate,
                "Which Angular feature helps prevent loading modules until needed?",
                "Eager loading", false, "Lazy loading", true, "Change detection", false, "View encapsulation", false);

            await AddQuestionIfMissing("Angular", QuestionLevel.Intermediate,
                "Which lifecycle hook runs after Angular initializes component input properties?",
                "ngOnInit", true, "ngOnDestroy", false, "ngAfterViewInit", false, "ngDoCheck", false);

            await AddQuestionIfMissing("Angular", QuestionLevel.Intermediate,
                "What does 'AsyncPipe' primarily do?",
                "Formats numbers", false, "Subscribes/unsubscribes from observables automatically", true, "Validates forms", false, "Compiles templates", false);

            await AddQuestionIfMissing("Angular", QuestionLevel.Professional,
                "Best way to avoid memory leaks with subscriptions in components?",
                "Never subscribe", false, "Use takeUntil + Subject in ngOnDestroy", true, "Use console.log", false, "Disable change detection", false);

            await AddQuestionIfMissing("Angular", QuestionLevel.Professional,
                "Which strategy reduces change detection checks for a component?",
                "Default", false, "OnPush", true, "ManualOnly", false, "ZoneLessOnly", false);

            await AddQuestionIfMissing("Angular", QuestionLevel.Professional,
                "What does Angular route resolver do?",
                "Creates routes", false, "Fetches data before route activates", true, "Styles routes", false, "Lazy loads CSS", false);

            await AddQuestionIfMissing("Angular", QuestionLevel.Professional,
                "In large apps, recommended state management option?",
                "Direct DOM manipulation", false, "NgRx/Signals based patterns", true, "Inline scripts", false, "Hardcoded JSON", false);

            await AddQuestionIfMissing("Angular", QuestionLevel.Professional,
                "What is a key benefit of standalone components?",
                "More CSS", false, "Less module boilerplate", true, "Slower builds", false, "Removes routing", false);

            await AddQuestionIfMissing("C#", QuestionLevel.Beginner,
                "Which keyword defines a class in C#?",
                "function", false, "class", true, "def", false, "structOnly", false);

            await AddQuestionIfMissing("C#", QuestionLevel.Beginner,
                "What is the correct entry point method signature in a console app?",
                "public void Main()", false, "static void Main(string[] args)", true, "int main()", false, "Main(): void", false);

            await AddQuestionIfMissing("C#", QuestionLevel.Beginner,
                "Which type is used for whole numbers?",
                "string", false, "int", true, "bool", false, "DateTime", false);

            await AddQuestionIfMissing("C#", QuestionLevel.Beginner,
                "What does 'using' statement commonly manage?",
                "Loops", false, "Disposing resources (IDisposable)", true, "Inheritance", false, "Serialization", false);

            await AddQuestionIfMissing("C#", QuestionLevel.Beginner,
                "Which keyword prevents a class from being inherited?",
                "sealed", true, "static", false, "virtual", false, "partial", false);

            await AddQuestionIfMissing("C#", QuestionLevel.Intermediate,
                "Which collection guarantees unique items?",
                "List<T>", false, "HashSet<T>", true, "Queue<T>", false, "Stack<T>", false);

            await AddQuestionIfMissing("C#", QuestionLevel.Intermediate,
                "What is LINQ primarily used for?",
                "UI design", false, "Querying collections/data", true, "Multithreading only", false, "Networking only", false);

            await AddQuestionIfMissing("C#", QuestionLevel.Intermediate,
                "What does 'async/await' help with?",
                "CPU overclock", false, "Non-blocking async operations", true, "Encrypt strings", false, "Avoid exceptions", false);

            await AddQuestionIfMissing("C#", QuestionLevel.Intermediate,
                "Which access modifier makes a member visible only within its class?",
                "public", false, "private", true, "internal", false, "protected internal", false);

            await AddQuestionIfMissing("C#", QuestionLevel.Intermediate,
                "Which is true about 'var'?",
                "var is dynamic always", false, "var is compile-time inferred type", true, "var is object always", false, "var disables typing", false);

            await AddQuestionIfMissing("C#", QuestionLevel.Professional,
                "What does 'IQueryable' enable compared to 'IEnumerable' in EF Core?",
                "In-memory execution always", false, "Query translation to SQL", true, "Faster UI rendering", false, "No execution", false);

            await AddQuestionIfMissing("C#", QuestionLevel.Professional,
                "What is the best practice for HttpClient usage?",
                "new HttpClient per call", false, "Use IHttpClientFactory", true, "Use WebClient", false, "Use Thread.Sleep", false);

            await AddQuestionIfMissing("C#", QuestionLevel.Professional,
                "What does 'ConfigureAwait(false)' do?",
                "Blocks thread", false, "Avoids capturing synchronization context", true, "Adds caching", false, "Forces UI update", false);

            await AddQuestionIfMissing("C#", QuestionLevel.Professional,
                "Which pattern is recommended for large apps to separate layers?",
                "Spaghetti code", false, "Clean Architecture", true, "Inline SQL only", false, "No DTOs", false);

            await AddQuestionIfMissing("C#", QuestionLevel.Professional,
                "In EF Core, how to avoid N+1 when loading navigation properties?",
                "Disable tracking", false, "Use Include/ThenInclude", true, "Use random()", false, "Use AsParallel()", false);

            await AddQuestionIfMissing("CSS", QuestionLevel.Beginner,
                "Which property changes text color?",
                "font-style", false, "color", true, "background", false, "display", false);

            await AddQuestionIfMissing("CSS", QuestionLevel.Beginner,
                "Which selector targets an element by id?",
                ".class", false, "#id", true, "tag", false, "*", false);

            await AddQuestionIfMissing("CSS", QuestionLevel.Beginner,
                "Which property controls element spacing outside the border?",
                "padding", false, "margin", true, "border-radius", false, "gap", false);

            await AddQuestionIfMissing("CSS", QuestionLevel.Beginner,
                "Which value makes an element a flex container?",
                "display: grid", false, "display: flex", true, "position: flex", false, "flex: display", false);

            await AddQuestionIfMissing("CSS", QuestionLevel.Beginner,
                "Which property changes background color?",
                "background-color", true, "text-color", false, "font-color", false, "color-bg", false);

            await AddQuestionIfMissing("CSS", QuestionLevel.Intermediate,
                "Which unit is relative to the root font-size?",
                "em", false, "rem", true, "px", false, "cm", false);

            await AddQuestionIfMissing("CSS", QuestionLevel.Intermediate,
                "What does 'box-sizing: border-box' do?",
                "Adds shadow", false, "Includes padding/border in width/height", true, "Centers element", false, "Makes text bold", false);

            await AddQuestionIfMissing("CSS", QuestionLevel.Intermediate,
                "Which property creates space between flex items?",
                "gap", true, "padding", false, "border", false, "outline", false);

            await AddQuestionIfMissing("CSS", QuestionLevel.Intermediate,
                "Which pseudo-class targets element on hover?",
                ":active", false, ":hover", true, ":focus", false, ":visited", false);

            await AddQuestionIfMissing("CSS", QuestionLevel.Intermediate,
                "Which layout is best for 2D grid systems?",
                "Flexbox", false, "CSS Grid", true, "Float", false, "Inline-block", false);

            await AddQuestionIfMissing("CSS", QuestionLevel.Professional,
                "What is specificity order (higher priority)?",
                "tag < class < id", true, "id < class < tag", false, "class < tag < id", false, "all equal", false);

            await AddQuestionIfMissing("CSS", QuestionLevel.Professional,
                "Which is best for responsive typography?",
                "fixed px everywhere", false, "clamp()", true, "only pt", false, "hard-coded rem only", false);

            await AddQuestionIfMissing("CSS", QuestionLevel.Professional,
                "What does 'position: sticky' require to work properly?",
                "display: flex", false, "a top/left/right/bottom offset", true, "box-shadow", false, "z-index: 0", false);

            await AddQuestionIfMissing("CSS", QuestionLevel.Professional,
                "Which method avoids layout shift for images?",
                "no width/height", false, "set width/height or aspect-ratio", true, "use border", false, "use float", false);

            await AddQuestionIfMissing("CSS", QuestionLevel.Professional,
                "What is a common performance improvement in animations?",
                "animate width/height", false, "animate transform/opacity", true, "animate border", false, "animate left/top", false);

            await AddQuestionIfMissing("HTML", QuestionLevel.Beginner,
                "What does HTML stand for?",
                "HyperText Markup Language", true, "HighText Markdown Language", false, "Hyper Tool Multi Language", false, "None", false);

            await AddQuestionIfMissing("HTML", QuestionLevel.Beginner,
                "Which tag creates a hyperlink?",
                "<link>", false, "<a>", true, "<href>", false, "<url>", false);

            await AddQuestionIfMissing("HTML", QuestionLevel.Beginner,
                "Which tag is used for an image?",
                "<img>", true, "<image>", false, "<pic>", false, "<src>", false);

            await AddQuestionIfMissing("HTML", QuestionLevel.Beginner,
                "Which tag is used for a form input?",
                "<input>", true, "<field>", false, "<text>", false, "<forminput>", false);

            await AddQuestionIfMissing("HTML", QuestionLevel.Beginner,
                "Which semantic tag represents navigation links?",
                "<nav>", true, "<section>", false, "<div>", false, "<header>", false);

            await AddQuestionIfMissing("HTML", QuestionLevel.Intermediate,
                "Which attribute links a label to an input?",
                "id", false, "for", true, "name", false, "data-label", false);

            await AddQuestionIfMissing("HTML", QuestionLevel.Intermediate,
                "What does the 'required' attribute do?",
                "Adds styles", false, "Prevents submit if empty", true, "Encrypts input", false, "Adds placeholder", false);

            await AddQuestionIfMissing("HTML", QuestionLevel.Intermediate,
                "Which input type is used for email validation?",
                "text", false, "email", true, "mail", false, "validate", false);

            await AddQuestionIfMissing("HTML", QuestionLevel.Intermediate,
                "Which tag is used for table row?",
                "<tr>", true, "<td>", false, "<th>", false, "<row>", false);

            await AddQuestionIfMissing("HTML", QuestionLevel.Intermediate,
                "Which tag is used to group form elements with a caption?",
                "<fieldset>", true, "<legendary>", false, "<group>", false, "<section>", false);

            await AddQuestionIfMissing("HTML", QuestionLevel.Professional,
                "What is the purpose of ARIA attributes?",
                "Improve CSS", false, "Improve accessibility for assistive tech", true, "Speed up JS", false, "Replace HTML", false);

            await AddQuestionIfMissing("HTML", QuestionLevel.Professional,
                "Which meta tag is important for responsive design?",
                "charset", false, "viewport", true, "author", false, "robots", false);

            await AddQuestionIfMissing("HTML", QuestionLevel.Professional,
                "What does 'defer' do on a script tag?",
                "Runs immediately", false, "Executes after HTML parsing", true, "Blocks rendering", false, "Disables JS", false);

            await AddQuestionIfMissing("HTML", QuestionLevel.Professional,
                "What is a correct use of <button type=\"submit\">?",
                "Inside a form to submit", true, "Only outside forms", false, "Only with href", false, "Only with image", false);

            await AddQuestionIfMissing("HTML", QuestionLevel.Professional,
                "Which attribute is used to open link in new tab securely?",
                "target=_blank only", false, "target=_blank + rel=\"noopener noreferrer\"", true, "rel=css", false, "noopener is invalid", false);

            await AddQuestionIfMissing("JavaScript", QuestionLevel.Beginner,
                "Which keyword declares a block-scoped variable?",
                "var", false, "let", true, "define", false, "dim", false);

            await AddQuestionIfMissing("JavaScript", QuestionLevel.Beginner,
                "Which method converts JSON string to object?",
                "JSON.stringify", false, "JSON.parse", true, "JSON.toObject", false, "parse.JSON", false);

            await AddQuestionIfMissing("JavaScript", QuestionLevel.Beginner,
                "What is the result type of 'typeof []'?",
                "array", false, "object", true, "list", false, "null", false);

            await AddQuestionIfMissing("JavaScript", QuestionLevel.Beginner,
                "Which symbol is used for strict equality?",
                "==", false, "===", true, "!=", false, "=", false);

            await AddQuestionIfMissing("JavaScript", QuestionLevel.Beginner,
                "Which function runs after a delay?",
                "setTimeout", true, "setNow", false, "delay()", false, "sleep()", false);

            await AddQuestionIfMissing("JavaScript", QuestionLevel.Intermediate,
                "Which array method creates a new array by transforming items?",
                "forEach", false, "map", true, "push", false, "splice", false);

            await AddQuestionIfMissing("JavaScript", QuestionLevel.Intermediate,
                "What does 'Promise.all' do?",
                "Runs promises sequentially", false, "Resolves when all resolve, rejects if any rejects", true, "Cancels promises", false, "Delays execution", false);

            await AddQuestionIfMissing("JavaScript", QuestionLevel.Intermediate,
                "What is event bubbling?",
                "Event stops at target", false, "Event propagates from child to parent", true, "Event goes only to window", false, "Event only on capture", false);

            await AddQuestionIfMissing("JavaScript", QuestionLevel.Intermediate,
                "What is a closure?",
                "A CSS feature", false, "Function + preserved lexical scope", true, "A loop", false, "A DOM node", false);

            await AddQuestionIfMissing("JavaScript", QuestionLevel.Intermediate,
                "Which keyword prevents reassignment of a variable binding?",
                "var", false, "const", true, "let", false, "static", false);

            await AddQuestionIfMissing("JavaScript", QuestionLevel.Professional,
                "What does the event loop manage?",
                "GPU rendering", false, "Async callbacks and task queues", true, "CSS parsing", false, "HTTP encryption", false);

            await AddQuestionIfMissing("JavaScript", QuestionLevel.Professional,
                "What is a common cause of memory leaks?",
                "Too many console.logs", false, "Unremoved event listeners / retained references", true, "Using let", false, "Using strict mode", false);

            await AddQuestionIfMissing("JavaScript", QuestionLevel.Professional,
                "Which is best for deep cloning in modern JS (with limitations)?",
                "Object.assign only", false, "structuredClone()", true, "JSON.parse(JSON.stringify()) always best", false, "clone()", false);

            await AddQuestionIfMissing("JavaScript", QuestionLevel.Professional,
                "What is tree-shaking?",
                "Runtime bundling", false, "Removing unused exports in bundling", true, "Minifying HTML only", false, "Caching API responses", false);

            await AddQuestionIfMissing("JavaScript", QuestionLevel.Professional,
                "Which is correct about module scope in ES modules?",
                "Everything is global", false, "Top-level is module-scoped", true, "Same as script tags always", false, "No imports allowed", false);

            await AddQuestionIfMissing("Python", QuestionLevel.Beginner,
                "Which keyword defines a function in Python?",
                "func", false, "def", true, "function", false, "lambda", false);

            await AddQuestionIfMissing("Python", QuestionLevel.Beginner,
                "Which data type is immutable?",
                "list", false, "tuple", true, "dict", false, "set", false);

            await AddQuestionIfMissing("Python", QuestionLevel.Beginner,
                "How do you start a comment line?",
                "//", false, "#", true, "/*", false, "<!--", false);

            await AddQuestionIfMissing("Python", QuestionLevel.Beginner,
                "Which function prints output?",
                "echo()", false, "print()", true, "console.log()", false, "out()", false);

            await AddQuestionIfMissing("Python", QuestionLevel.Beginner,
                "Which operator is used for exponentiation?",
                "^", false, "**", true, "*", false, "//", false);

            await AddQuestionIfMissing("Python", QuestionLevel.Intermediate,
                "What does list comprehension produce?",
                "A generator always", false, "A list", true, "A dict", false, "A tuple", false);

            await AddQuestionIfMissing("Python", QuestionLevel.Intermediate,
                "Which keyword handles exceptions?",
                "catch", false, "except", true, "rescue", false, "error", false);

            await AddQuestionIfMissing("Python", QuestionLevel.Intermediate,
                "What does 'pip' primarily do?",
                "Runs scripts", false, "Installs packages", true, "Formats code", false, "Compiles Python", false);

            await AddQuestionIfMissing("Python", QuestionLevel.Intermediate,
                "Which statement opens a file safely?",
                "open()", false, "with open(...) as f", true, "file.open()", false, "readfile()", false);

            await AddQuestionIfMissing("Python", QuestionLevel.Intermediate,
                "What does 'None' represent?",
                "zero", false, "absence of value", true, "empty string", false, "false always", false);

            await AddQuestionIfMissing("Python", QuestionLevel.Professional,
                "What is a virtual environment used for?",
                "Speed up CPU", false, "Isolate dependencies per project", true, "Encrypt code", false, "Compile to C", false);

            await AddQuestionIfMissing("Python", QuestionLevel.Professional,
                "What does GIL affect in CPython?",
                "Disk IO", false, "True parallel CPU threads", true, "HTTP requests", false, "JSON parsing", false);

            await AddQuestionIfMissing("Python", QuestionLevel.Professional,
                "What is a generator?",
                "Always a list", false, "Produces values lazily using yield", true, "A dict type", false, "A class decorator", false);

            await AddQuestionIfMissing("Python", QuestionLevel.Professional,
                "Which improves type checking in Python projects?",
                "mypy/pyright", true, "pip only", false, "numpy", false, "pytest only", false);

            await AddQuestionIfMissing("Python", QuestionLevel.Professional,
                "Which is best for structured data validation in Python APIs?",
                "string concatenation", false, "Pydantic", true, "print()", false, "random()", false);

            await AddQuestionIfMissing("React", QuestionLevel.Beginner,
                "What does JSX compile to?",
                "SQL", false, "JavaScript function calls", true, "CSS", false, "HTML only", false);

            await AddQuestionIfMissing("React", QuestionLevel.Beginner,
                "Which hook manages component state?",
                "useEffect", false, "useState", true, "useMemo", false, "useRef", false);

            await AddQuestionIfMissing("React", QuestionLevel.Beginner,
                "Which prop is required for lists?",
                "index", false, "key", true, "id", false, "name", false);

            await AddQuestionIfMissing("React", QuestionLevel.Beginner,
                "How do you pass data from parent to child?",
                "props", true, "state only", false, "context only", false, "router", false);

            await AddQuestionIfMissing("React", QuestionLevel.Beginner,
                "Which hook runs side effects?",
                "useState", false, "useEffect", true, "useCallback", false, "useId", false);

            await AddQuestionIfMissing("React", QuestionLevel.Intermediate,
                "What does useMemo do?",
                "Always rerenders", false, "Memoizes computed value", true, "Manages routes", false, "Fetches data", false);

            await AddQuestionIfMissing("React", QuestionLevel.Intermediate,
                "What does useCallback do?",
                "Memoizes a function reference", true, "Stores DOM", false, "Creates state", false, "Stops renders", false);

            await AddQuestionIfMissing("React", QuestionLevel.Intermediate,
                "Which is correct about controlled components?",
                "Input manages itself", false, "Value comes from state", true, "Must use refs", false, "Only for forms", false);

            await AddQuestionIfMissing("React", QuestionLevel.Intermediate,
                "What is lifting state up?",
                "Moving state to child", false, "Moving state to common parent", true, "Using context", false, "Using redux", false);

            await AddQuestionIfMissing("React", QuestionLevel.Intermediate,
                "Which API is used for navigation in React Router v6?",
                "useHistory", false, "useNavigate", true, "history.pushState", false, "navigateTo", false);

            await AddQuestionIfMissing("React", QuestionLevel.Professional,
                "What does React StrictMode do in dev?",
                "Removes warnings", false, "Double-invokes some lifecycles/effects to find issues", true, "Speeds production", false, "Disables hooks", false);

            await AddQuestionIfMissing("React", QuestionLevel.Professional,
                "What is a common cause of infinite re-render?",
                "Using CSS", false, "Calling setState in render without condition", true, "Using useRef", false, "Using memo", false);

            await AddQuestionIfMissing("React", QuestionLevel.Professional,
                "Best practice for expensive list rendering?",
                "Render all always", false, "Virtualization (react-window)", true, "Use alerts", false, "Use tables only", false);

            await AddQuestionIfMissing("React", QuestionLevel.Professional,
                "What is the recommended way to fetch data in effects?",
                "Directly in render", false, "Inside useEffect with cleanup/cancellation", true, "Only in constructor", false, "Only in reducers", false);

            await AddQuestionIfMissing("React", QuestionLevel.Professional,
                "Why use React Query / SWR?",
                "For CSS themes", false, "Caching + retries + deduping + invalidation", true, "For routing", false, "To remove hooks", false);

            await AddQuestionIfMissing("TypeScript", QuestionLevel.Beginner,
                "What does TypeScript add to JavaScript?",
                "CSS", false, "Static typing", true, "SQL", false, "DOM only", false);

            await AddQuestionIfMissing("TypeScript", QuestionLevel.Beginner,
                "Which type represents a true/false value?",
                "string", false, "boolean", true, "number", false, "any", false);

            await AddQuestionIfMissing("TypeScript", QuestionLevel.Beginner,
                "How do you define an array of numbers?",
                "number[]", true, "int[]", false, "array<number>", false, "numbers()", false);

            await AddQuestionIfMissing("TypeScript", QuestionLevel.Beginner,
                "What does 'any' mean?",
                "Strictly typed", false, "Opt-out of type checking", true, "Only numbers", false, "Only strings", false);

            await AddQuestionIfMissing("TypeScript", QuestionLevel.Beginner,
                "Which keyword defines an interface?",
                "type", false, "interface", true, "class", false, "struct", false);

            await AddQuestionIfMissing("TypeScript", QuestionLevel.Intermediate,
                "What is a union type?",
                "One fixed type", false, "A type that can be one of several", true, "A class", false, "A namespace", false);

            await AddQuestionIfMissing("TypeScript", QuestionLevel.Intermediate,
                "What does 'unknown' require before usage?",
                "Nothing", false, "Type narrowing/checks", true, "Casting to any", false, "Compilation flag", false);

            await AddQuestionIfMissing("TypeScript", QuestionLevel.Intermediate,
                "What is generics used for?",
                "Hardcoding types", false, "Type-safe reusable components/functions", true, "Only enums", false, "Only interfaces", false);

            await AddQuestionIfMissing("TypeScript", QuestionLevel.Intermediate,
                "Which helps ensure a string is one of known values?",
                "enum / string literal union", true, "any", false, "unknown", false, "void", false);

            await AddQuestionIfMissing("TypeScript", QuestionLevel.Intermediate,
                "What does 'as const' do?",
                "Makes value mutable", false, "Narrows literals to readonly literal types", true, "Disables TS", false, "Adds runtime checks", false);

            await AddQuestionIfMissing("TypeScript", QuestionLevel.Professional,
                "What does conditional type allow?",
                "Runtime if", false, "Type-level branching", true, "CSS conditions", false, "SQL joins", false);

            await AddQuestionIfMissing("TypeScript", QuestionLevel.Professional,
                "Why prefer 'unknown' over 'any'?",
                "unknown is faster", false, "unknown forces type safety", true, "any is safer", false, "unknown removes types", false);

            await AddQuestionIfMissing("TypeScript", QuestionLevel.Professional,
                "What is utility type 'Partial<T>'?",
                "Makes properties required", false, "Makes all properties optional", true, "Removes properties", false, "Converts to array", false);

            await AddQuestionIfMissing("TypeScript", QuestionLevel.Professional,
                "How to type an API response generically?",
                "Use var", false, "Use generics like ApiResponse<T>", true, "Use only any", false, "No typing", false);

            await AddQuestionIfMissing("TypeScript", QuestionLevel.Professional,
                "Which is best to model DTOs in frontend?",
                "string-only", false, "interfaces/types + runtime validation if needed", true, "no types", false, "inline JSON only", false);

            await context.SaveChangesAsync();
        }
    }
}
 