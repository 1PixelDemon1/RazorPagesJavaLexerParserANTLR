function renderTree(node, container, level = 0) {
    var ul = document.createElement('ul');
    var li = document.createElement('li');
    li.textContent = node.Text;
    li.dataset.level = level;

    if (node.Children && node.Children.length > 0) {
        li.classList.add('collapsible'); // Добавляем класс только для сворачиваемых узлов
        li.addEventListener('click', function (event) {
            event.stopPropagation();
            var childrenContainer = this.querySelector('div');
            if (childrenContainer != null) {
                childrenContainer.style.display = childrenContainer.style.display === 'none' ? 'block' : 'none';
            }
        });

        var childrenContainer = document.createElement('div');
        childrenContainer.style.marginLeft = '3px';
        li.appendChild(childrenContainer);

        for (var i = 0; i < node.Children.length; i++) {
            renderTree(node.Children[i], childrenContainer, level + 1);
        }
    }

    ul.appendChild(li);
    container.appendChild(ul);
}