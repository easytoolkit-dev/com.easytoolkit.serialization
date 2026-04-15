using System;

namespace EasyToolkit.Serialization.Processors
{
    /// <summary>
    /// Specifies that a field or property should receive a <see cref="ISerializationProcessor"/> dependency injection.
    /// </summary>
    /// <remarks>
    /// <para>
    /// This attribute supports dynamic processor type selection through the <see cref="CandidateTypesGetter"/> property.
    /// When specified, candidate processor types are determined at runtime by evaluating the expression path against
    /// the containing processor instance. The system then selects the most appropriate processor from these candidates
    /// based on type matching rules.
    /// </para>
    /// <para>
    /// <b>Expression Path Syntax:</b>
    /// <list type="bullet">
    /// <item><description>Direct member: "CandidateProcessorTypes"</description></item>
    /// <item><description>Nested access: "Config.AllowedProcessors"</description></item>
    /// <item><description>Method call: "GetCandidateProcessors()"</description></item>
    /// </list>
    /// </para>
    /// </remarks>
    /// <example>
    /// <code>
    /// // Example 1: Auto-detect processor type from field type (default behavior)
    /// [DependencyProcessor]
    /// private ISerializationProcessor&lt;string&gt; _stringProcessor;
    ///
    /// // Example 2: Specify candidate processor types
    /// [DependencyProcessor(CandidateTypesGetter = nameof(GetCandidateProcessors))]
    /// private ISerializationProcessor _processor;
    ///
    /// private IEnumerable&lt;Type&gt; GetCandidateProcessors()
    /// {
    ///     yield return typeof(StringProcessor);
    ///     yield return typeof(CustomStringProcessor);
    /// }
    ///
    /// // Example 3: Specify candidates with exclusions
    /// [DependencyProcessor(
    ///     CandidateTypesGetter = nameof(AllProcessors),
    ///     ExcludedTypesGetter = nameof(ExcludedProcessors))]
    /// private ISerializationProcessor&lt;List&lt;int&gt;&gt; _listProcessor;
    ///
    /// private static IEnumerable&lt;Type&gt; AllProcessors => new[]
    /// {
    ///     typeof(ListProcessor&lt;&gt;),
    ///     typeof(ArrayProcessor),
    ///     typeof(CustomListProcessor&lt;&gt;)
    /// };
    ///
    /// private static IEnumerable&lt;Type&gt; ExcludedProcessors => new[]
    /// {
    ///     typeof(ArrayProcessor) // Exclude array processor
    /// };
    /// </code>
    /// </example>
    [AttributeUsage(AttributeTargets.Field | AttributeTargets.Property)]
    public class DependencyProcessorAttribute : Attribute
    {
        /// <summary>
        /// Gets or sets the expression path for dynamically obtaining candidate processor types.
        /// </summary>
        /// <value>
        /// A string representing an expression path that evaluates to an <c>IEnumerable&lt;Type&gt;</c>,
        /// the system will select the most appropriate processor from the candidates.
        /// </value>
        /// <remarks>
        /// When set, candidate processor types are determined by evaluating this expression path
        /// against the containing processor instance. If null or empty, all available processor types
        /// are considered as candidates.
        /// </remarks>
        public string CandidateTypesGetter { get; set; }

        /// <summary>
        /// Gets or sets the expression path for dynamically obtaining excluded processor types.
        /// </summary>
        /// <value>
        /// A string representing an expression path that evaluates to an <c>IEnumerable&lt;Type&gt;</c>
        /// containing types to exclude from selection.
        /// </value>
        /// <remarks>
        /// When set, these types will be excluded from the candidate processor types before
        /// type matching occurs. This is useful when you want to prevent certain processors
        /// from being used while still considering the rest.
        /// </remarks>
        public string ExcludedTypesGetter { get; set; }
    }
}
